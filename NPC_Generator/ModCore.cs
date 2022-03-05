using System.Linq;
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
            DontDestroyOnLoad(RootGOHolder);
            RootGOHolder.SetActive(false);
        }

        [HarmonyPatch(typeof(Game), nameof(Game.Awake))]
        public static class GamestartPatch
        {
            public static void Postfix(Game __instance)
            {
                NpcManager.SetupNPC();
                NpcManager.Init();
            }
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        [HarmonyPriority(Priority.Last)]
        public static class ZNSAwakepatch
        {
            public static void Prefix(ZNetScene __instance)
            {
                if (__instance.m_prefabs.Count <= 0) return;
                
                if(NetworkedNPC != null) __instance.m_prefabs.Add(NetworkedNPC);
             }

            public static void Postfix(ZNetScene __instance)
            {
                NpcManager.ZNSStuff();
               var test = __instance.m_prefabs.Find(x => x.name == "BasicHuman");
               __instance.m_prefabs.Remove(test);
               __instance.m_prefabs.Add(NetworkedNPC);
            }
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.OnDestroy))]
        public static class onZnetDestroy
        {
            public static void Postfix()
            {
                foreach (Transform VARIABLE in RootGOHolder.transform)
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
