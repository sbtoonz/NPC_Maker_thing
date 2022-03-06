﻿using System;
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
                __instance.m_prefabs.Add(NPC_Generator.NetworkedNPC);
                
            }
        }

        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
        public static class ObjectDBPatch
        {
            public static void Postfix(ObjectDB __instance)
            {
                NPC_Utilities.BuildHumanNPC();
                File.SetLastWriteTime(NPC_Generator.Paths + "/npc_config.yml", DateTime.UtcNow);
                if(__instance.m_items.Count<=0 || __instance.GetItemPrefab("Wood") == null) return;
                var go = ZNetScene.instance.GetPrefab("BasicHuman");
                NPC_Human.SetupArmor(go);
            }
        }
    }
}