using System;
using System.IO;
using HarmonyLib;
using NPC_Generator.NPC_Utilities;

namespace NPC_Generator.Patches
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
        [HarmonyAfter("org.bepinex.helpers.ItemManager")]
        public static class ObjectDBPatch
        {
            public static void Postfix(ObjectDB __instance)
            {
                NpcUtilities.BuildMaleHumanNpc();
                NpcUtilities.BuildFemaleHumanNpc();
            }
        }

        [HarmonyPatch(typeof(Localize), nameof(Localize.Awake))]
        public static class LocalizationPatch
        {
            public static void Prefix()
            {
                foreach (var KP in NPC_Names.EnglishNames)
                {
                    Localization.instance.AddWord(KP.Key, KP.Value);
                }
            }
        }
    }
}