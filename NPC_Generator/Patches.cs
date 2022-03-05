using HarmonyLib;

namespace NPC_Generator
{
    public class Patches
    {
        [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Start))]
        public static class TestPatch
        {
            public static void Postfix(FejdStartup __instance)
            {
                NPC_Utilities.BuildHumanNPC();
            }
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        public static class ZNSAwakePatch
        {
            public static void Prefix(ZNetScene __instance)
            {
                if(__instance.m_prefabs.Count <= 0) return;
                __instance.m_prefabs.Add(NPC_Generator.NetworkedNPC);
                
            }
        }
    }
}