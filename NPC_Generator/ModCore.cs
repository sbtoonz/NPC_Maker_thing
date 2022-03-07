using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
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

        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            harmony = new(ModGUID);
            harmony.PatchAll(assembly);
            RootGOHolder = new GameObject("NPC");
            DontDestroyOnLoad(RootGOHolder);
            RootGOHolder.SetActive(false);
            _serverConfigLocked = config("General", "Lock Configuration", false, "Lock Configuration");
            configSync.AddLockingConfigEntry(_serverConfigLocked);
            if (!File.Exists(Paths + "/npc_config.yml"))
            {
                File.Create(Paths + "/npc_config.yml").Close();
            }
            ReadYamlConfigFile(null!, null!);
            NpcConfig.ValueChanged += OnValueChangedNPConfig;
            SetupWatcher();
            
        }
        private static void OnValueChangedNPConfig()
        {
            if (ZNetScene.instance == null)
            {
                ZNetScene? Zscene = null;
                foreach (var gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
                {
                    if (gameObject.name == "_GameMain")
                    {
                        Zscene = gameObject.GetComponent<ZNetScene>();
                    }
                }
                foreach (var KP in NpcConfig.Value)
                {
                    if (Zscene?.m_prefabs.Find(x => x.name == KP.Key))
                    {
                        var prefab = Zscene?.GetPrefab(KP.Key);
                        Zscene?.m_prefabs.Remove(prefab);
                        var tempNPC = NPC_Human.ReturnNamedNpc(KP.Key, KP.Value, Zscene!);
                        Zscene?.m_prefabs.Add(tempNPC);
                        Zscene?.m_namedPrefabs.Remove(KP.Key.GetStableHashCode(), out prefab);
                        Zscene?.m_namedPrefabs.Add(tempNPC.name.GetStableHashCode(), tempNPC);
                    }
                    else
                    {
                        var tempNPC = NPC_Human.ReturnNamedNpc(KP.Key,KP.Value, Zscene!);
                        Zscene?.m_prefabs.Add(tempNPC);
                        Zscene?.m_namedPrefabs.Add(tempNPC.name.GetStableHashCode(), tempNPC);
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
            catch
            {
                Debug.LogError("There was an issue loading your npc_config.yml");
                Debug.LogError($"Please check your config entries for spelling and format!");
            }
            
        }
    }
}
