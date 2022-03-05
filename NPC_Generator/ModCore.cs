using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace NPC_Generator
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class NPC_Generator : BaseUnityPlugin
    {
        private const string ModName = "NPC_Maker";
        private const string ModVersion = "1.0";
        private const string ModGUID = "com.odinplus.NPC_Maker";
        private static Harmony harmony = null!;
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

        [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Start))]
        public static class TestPatch
        {
            public static void Postfix(FejdStartup __instance)
            {
                BuildHumanNPC();
            }
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        public static class ZNSAwakePatch
        {
            public static void Prefix(ZNetScene __instance)
            {
                if(__instance.m_prefabs.Count <= 0) return;
                __instance.m_prefabs.Add(NetworkedNPC);
                
            }
        }

        private static void BuildHumanNPC()
        {
            var temp = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject? tempplayer = null;
            foreach (var o in temp)
            {
                if (o.GetComponent<Player>() != null)
                { 
                    NetworkedNPC = Instantiate(o, RootGOHolder?.transform!);
                    tempplayer = o;
                    break;
                }
            }
            DestroyImmediate(NetworkedNPC?.GetComponent<PlayerController>());
            DestroyImmediate(NetworkedNPC?.GetComponent<Player>());
            DestroyImmediate(NetworkedNPC?.GetComponent<Talker>());
            DestroyImmediate(NetworkedNPC?.GetComponent<Skills>());
            NetworkedNPC!.name = "Basic Human";
            var basicznet =
                NetworkedNPC.GetComponent<ZNetView>();
            basicznet.enabled = true;
            NetworkedNPC.GetComponent<ZSyncAnimation>().enabled = true;
            NetworkedNPC.GetComponent<ZSyncTransform>().enabled = true;
            var HumanoidAI = NetworkedNPC.AddComponent<Humanoid>();
            HumanoidAI.m_nview = basicznet;
            basicznet.m_persistent = true;
            HumanoidAI.CopyChildrenComponents<Humanoid, Player>(tempplayer!.GetComponent<Player>());
            NetworkedNPC.AddComponent<Tameable>();
            NetworkedNPC.name = "BasicHuman";
            NetworkedNPC.transform.name = "BasicHuman";
            NetworkedNPC.transform.position = Vector3.zero;
            //go.AddComponent<NPC_Human>();
            var MonsterAI =
                NetworkedNPC.AddComponent<MonsterAI>();
            SetupMonsterAI(MonsterAI);
            MonsterAI.m_nview = NetworkedNPC.GetComponent<ZNetView>();
            MonsterAI.m_nview.m_zdo = new ZDO();
            var hum = NetworkedNPC.GetComponent<Humanoid>();
            hum.m_faction = Character.Faction.PlainsMonsters;
            hum.m_health = 200;
            hum.m_defaultItems = new GameObject[0];
            hum.m_eye = NetworkedNPC.transform.Find("EyePos");
        }
        private static void SetupMonsterAI(MonsterAI ai)
        {
            ai.m_viewRange = 30;
            ai.m_viewAngle = 90;
            ai.m_hearRange = 9999;
            ai.m_alertedEffects.m_effectPrefabs = new EffectList.EffectData[0];
            ai.m_idleSound.m_effectPrefabs = new EffectList.EffectData[0];
            ai.m_pathAgentType = Pathfinding.AgentType.Humanoid;
            ai.m_smoothMovement = true;
            ai.m_jumpInterval = 20;
            ai.m_randomCircleInterval = 2;
            ai.m_randomMoveInterval = 30;
            ai.m_randomMoveRange = 3;
            ai.m_alertRange = 20;
            ai.m_fleeIfHurtWhenTargetCantBeReached = true;
            ai.m_fleeIfLowHealth = 0;
            ai.m_circulateWhileCharging = true;
            ai.m_enableHuntPlayer = true;
            ai.m_wakeupRange = 5;
            ai.m_wakeupEffects.m_effectPrefabs = new EffectList.EffectData[0];
        }
        public void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
