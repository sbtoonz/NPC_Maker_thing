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
        
        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            harmony = new(ModGUID);
            harmony.PatchAll(assembly);
            RootGOHolder = new GameObject("NPC");
            RootGOHolder.AddComponent<NpcManager>();
            DontDestroyOnLoad(RootGOHolder);
        }

        [HarmonyPatch(typeof(Game), nameof(Game.Awake))]
        public static class testpatch
        {
            public static void Postfix(Game __instance)
            {
                RootGOHolder.transform.SetParent(__instance.transform);
            }
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        public static class ZNSAwakepatch
        {
            public static void Postfix(ZNetScene __instance)
            {
                RootGOHolder.GetComponent<NpcManager>().SpawnHuman();
            }
        }

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.InputText))]
        public static class TerminalPatch
        {
            public static bool Prefix(Terminal __instance)
            {
                string phrase = __instance.m_input.text;
                if (phrase.ToLower() == "gimmehumoon")
                {
                    NpcManager.instance.SpawnNPC();
                    return false;
                }

                return true;
            }
        }
        
        public void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
