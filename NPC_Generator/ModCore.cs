#define BPM
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NPC_Generator.NPC_Utilities;
using NPC_Generator.Tools;
using ServerSync;
using UnityEngine;

namespace NPC_Generator
{
    
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class NPC_Generator : BaseUnityPlugin
    {
        internal const string ModName = "NPC_Maker";
        internal const string ModVersion = "0.0.1";
        private const string ModGUID = "com.odinplus.NPC_Maker";
        private static Harmony harmony = null!;
        internal static GameObject? RootGOHolder;
        internal static GameObject? NetworkedNPCMale;
        internal static GameObject? NetworkedNPCFemale;
        internal static ConfigEntry<bool>? _serverConfigLocked;
        internal static ConfigEntry<DebugLevel> _DebugLevel;
        internal static ManualLogSource npcLogger;
        internal static bool BadgerPlayerMeshMod;
        //badgershit
        public static Texture2D noMetal = null;
        public static Texture maleSkin = null;
        public static Texture maleSkinBump = null;
        public static Texture femaleSkin = null;
        public static Texture femaleSkinBump = null;
        public static SkinnedMeshRenderer maleSmr = null;
        public static SkinnedMeshRenderer femaleSmr = null;
        public static GameObject playerMale = null;
        public static GameObject playerFemale = null;
        
        
        private static ConfigSync configSync = new(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion};
        
        internal static readonly string Paths = BepInEx.Paths.ConfigPath;
        
        public static readonly CustomSyncedValue<Dictionary<string, NPCYamlConfig>> NpcConfig = 
            new(configSync, "npc config", new Dictionary<string, NPCYamlConfig>());
        private static Dictionary<string, NPCYamlConfig> entry_ { get; set; } = null!;
        ConfigEntry<T> config<T>(string group, string configName, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = Config.Bind(group, configName, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        ConfigEntry<T> config<T>(string group, string configName, T value, string description, bool synchronizedSetting = true) => config(group, configName, value, new ConfigDescription(description), synchronizedSetting);

        public enum DebugLevel
        {
            None,
            Some,
            All
        }
        public void Awake()
        {
            npcLogger = this.Logger;
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            harmony = new(ModGUID);
            harmony.PatchAll(assembly);
            RootGOHolder = new GameObject("NPC");
            DontDestroyOnLoad(RootGOHolder);
            RootGOHolder.SetActive(false);
            _serverConfigLocked = config("General", "Lock Configuration", false, "Lock Configuration");
            _DebugLevel = config("General", "Log level", DebugLevel.All, "This is how much debug info the mod gives");
            configSync.AddLockingConfigEntry(_serverConfigLocked);
            if (!File.Exists(Paths + "/npc_config.yml"))
            {
                File.Create(Paths + "/npc_config.yml").Close();
            }
            ReadYamlConfigFile(null!, null!);
            NpcConfig.ValueChanged += OnValueChangedNPConfig;
            SetupWatcher();
            

        }
        public SyncedList SyncedListTest { get; set; }

        public void Start()
        {
            try
            {
                var equip = Chainloader.PluginInfos.First(p => p.Key == "Badgers.BetterPlayerMesh");
                if (equip.Value.Instance != null)
                {
                    BadgerPlayerMeshMod = true;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            foreach (var gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name == "_GameMain")
                {
                    Zscene = gameObject.GetComponent<ZNetScene>();
                }
            }

        }
        public static ZNetScene Zscene { get; set; }


        internal static void assignZnet()
        {
            foreach (var gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name == "_GameMain")
                {
                    Zscene = gameObject.GetComponent<ZNetScene>();
                }
            }
        }
        internal static void DebugLog(DebugLevel level,string debugText)
        {
            switch (_DebugLevel.Value)
            {
                case DebugLevel.All:
                    switch (level)
                    {
                        case DebugLevel.Some:
                            npcLogger.Log(LogLevel.All, debugText);
                            break;
                        case DebugLevel.All:
                            npcLogger.LogWarning(debugText);
                            break;
                        case DebugLevel.None:
                            break;
                        default:
                            break;
                    }
                    break;
                case DebugLevel.None:
                    break;
                case DebugLevel.Some:
                    switch (level)
                    {
                        case DebugLevel.Some:
                            npcLogger.Log(LogLevel.All, debugText);
                            break;
                        case DebugLevel.All:
                            break;
                        case DebugLevel.None:
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        private static void OnValueChangedNPConfig()
        {
            if (ZNetScene.instance == null)
            {
                if (Zscene == null)
                {
                    assignZnet();

                }
                foreach (var KP in NpcConfig.Value)
                {
                    if (Zscene.m_prefabs.Find(x => x.name == KP.Key))
                    {
                        var prefab = Zscene.GetPrefab(KP.Key);
                        Zscene.m_prefabs.Remove(prefab);
                        var tempNPC = NPC_Human.ReturnNamedNpc(KP.Key, KP.Value, Zscene!);
                        Zscene.m_prefabs.Add(tempNPC);
                        Zscene.m_namedPrefabs.Remove(KP.Key.GetStableHashCode(), out prefab);
                        Zscene.m_namedPrefabs.Add(tempNPC.name.GetStableHashCode(), tempNPC);
                    }
                    else
                    {
                        var tempNpc = NPC_Human.ReturnNamedNpc(KP.Key,KP.Value, Zscene!);
                        Zscene.m_prefabs.Add(tempNpc);
                        Zscene.m_namedPrefabs.Add(tempNpc.name.GetStableHashCode(), tempNpc);
                    }
                    
                }  
            }
            else
            {
                foreach (var KP in NpcConfig.Value)
                {
                    if (ZNetScene.instance.m_prefabs.Find(x => x.name == KP.Key))
                    {
                        var prefab = ZNetScene.instance.GetPrefab(KP.Key);
                        ZNetScene.instance.m_prefabs.Remove(prefab);
                        var newNPC = NPC_Human.ReturnNamedNpc(KP.Key,KP.Value, ZNetScene.instance);
                        ZNetScene.instance.m_prefabs.Add(newNPC);
                        ZNetScene.instance.m_namedPrefabs.Remove(KP.Key.GetStableHashCode(), out prefab);
                        ZNetScene.instance.m_namedPrefabs.Add(newNPC.name.GetStableHashCode(), newNPC);
                    }
                    else
                    {
                        var tempNPC = NPC_Human.ReturnNamedNpc(KP.Key,KP.Value, ZNetScene.instance);
                        ZNetScene.instance.m_prefabs.Add(tempNPC);
                        ZNetScene.instance.m_namedPrefabs.Add(tempNPC.name.GetStableHashCode(), tempNPC);
                    }
                   
                }  
            }
        }
        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths, "npc_config.yml");
            watcher.Changed += ReadYamlConfigFile;
            watcher.Created += ReadYamlConfigFile;
            watcher.Renamed += ReadYamlConfigFile;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }
        private void ReadYamlConfigFile(object sender, FileSystemEventArgs e)
        {
            try
            {
                var file = File.OpenText(NPC_Generator.Paths + "/npc_config.yml");
                entry_ = YMLParser.ReadSerializedData(file.ReadToEnd());
                file.Close();
                NpcConfig.AssignLocalValue(entry_);
            }
            catch(Exception exception)
            {
                DebugLog(DebugLevel.Some,"There was an issue loading your npc_config.yml");
                DebugLog(DebugLevel.Some,$"Please check your config entries for spelling and format!");
                DebugLog(DebugLevel.All, exception.ToString());
            }
            
        }
    }
}
