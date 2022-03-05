using System.Reflection;
using BepInEx;
using HarmonyLib;
using NPC_Generator.NPC_Classes;
using UnityEngine;

namespace NPC_Generator
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class NPC_Generator : BaseUnityPlugin
    {
        private const string ModName = "New Mod";
        private const string ModVersion = "1.0";
        private const string ModGUID = "some.new.guid";
        private static Harmony harmony = null!;
        internal static GameObject? npc_prefab;
        internal static GameObject? RootGOHolder;
        internal static GameObject? NetworkedNPC;
        
        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            harmony = new(ModGUID);
            harmony.PatchAll(assembly);
            RootGOHolder = new GameObject("NPC");
            RootGOHolder.AddComponent<NpcManager>();
            DontDestroyOnLoad(RootGOHolder);
        }
        

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        public static class ZNSAwakepatch
        {
            public static void Prefix(ZNetScene __instance)
            {
                if (__instance.m_prefabs.Count <= 0) return;
                var go = Instantiate(Game.instance.m_playerPrefab, NpcManager.PrefabParent.transform);
                DestroyImmediate(go.GetComponent<PlayerController>());
                DestroyImmediate(go.GetComponent<Player>());
                DestroyImmediate(go.GetComponent<Talker>());
                DestroyImmediate(go.GetComponent<Skills>());
                go.name = "Basic Human";
                var basicznet =
                    go.GetComponent<ZNetView>();
                var HumanoidAI = go.AddComponent<Humanoid>();
                HumanoidAI.m_nview = basicznet;
                basicznet.m_persistent = true;
                HumanoidAI.CopyChildrenComponents<Humanoid, Player>(Game.instance.m_playerPrefab.GetComponent<Player>());
                 go.AddComponent<Tameable>();
                 go.name = "BasicHuman";
                go.transform.name = "BasicHuman";
                go.transform.position = Vector3.zero;
                go.AddComponent<NPC_Human>();
                NpcManager.BasicHuman = go;
                NpcManager.BasicHuman.name = "BasicHuman";
                NetworkedNPC = go;
                if(NetworkedNPC != null) __instance.m_prefabs.Add(NetworkedNPC);
             }
            

            public static void Postfix(ZNetScene __instance)
            {
                NpcManager.instance.Init();
            }
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.OnDestroy))]
        public static class onZnetDestroy
        {
            public static void Postfix()
            {
                foreach (Transform VARIABLE in NpcManager.PrefabParent.transform)
                {
                    Destroy(VARIABLE.gameObject);
                }
            }
        }
        public void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
