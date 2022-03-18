using System;
using BepInEx;
using HarmonyLib;
using NPC_Friendlies.MobAIBehaviors;
using NPC_Generator.MonoScripts.Villagers;
using RagnarsRokare.MobAI;
using UnityEngine;

namespace NPC_Friendlies;

public class Patches
{
    [HarmonyPatch(typeof(MonsterAI), "MakeTame")]
    static class MonsterAI_MakeTame_Patch
    {
        static void Postfix(MonsterAI __instance, ZNetView ___m_nview, Character ___m_character)
        {
                if (__instance.gameObject.GetComponent<VillagerBase>() == null) return;
                var mobInfo = MobAIBehaviorGather.MobConfig();
                __instance.m_consumeItems.Clear();
                __instance.m_consumeItems.AddRange(mobInfo.PostTameConsumables);
                __instance.m_consumeSearchRange = 50;
                ___m_character.m_faction = Character.Faction.Players;
                try
                {
                    var uniqueId = ___m_nview.GetZDO().GetString("RR_CharId", "");
                    if (uniqueId.IsNullOrWhiteSpace())
                    {
                        uniqueId = Guid.NewGuid().ToString();
                        ___m_nview.GetZDO().Set("RR_CharID", uniqueId);
                    }
                    MobManager.RegisterMob(___m_character, uniqueId, mobInfo.AIType, mobInfo.AIConfig);
                }
                catch (ArgumentException e)
                {
                    Debug.LogError($"Failed to register Mob AI ({mobInfo.AIType}). {e.Message}");
                    return;
                }
            
        }
    }
}