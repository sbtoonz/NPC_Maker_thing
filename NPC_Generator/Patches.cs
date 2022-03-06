using System;
using System.IO;
using HarmonyLib;
using UnityEngine;

namespace NPC_Generator
{
    public class Patches
    {
        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        public static class ZNSAwakePatch
        {
            public static void Prefix(ZNetScene __instance)
            {
                if(__instance.m_prefabs.Count <= 0) return;
                __instance.m_prefabs.Add(NPC_Generator.NetworkedNPCMale);
                __instance.m_prefabs.Add(NPC_Generator.NetworkedNPCFemale);
                
            }
        }

        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
        [HarmonyPriority(Priority.Last)]
        public static class ObjectDBPatch
        {
            public static void Postfix(ObjectDB __instance)
            {
                NPC_Utilities.BuildMaleHumanNpc();
                NPC_Utilities.BuildFemaleHumanNpc();
                File.SetLastWriteTime(NPC_Generator.Paths + "/npc_config.yml", DateTime.UtcNow);
            }
        }
    }
}