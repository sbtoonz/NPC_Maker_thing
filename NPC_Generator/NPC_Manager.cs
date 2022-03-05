using System;
using System.Collections.Generic;
using System.Linq;
using NPC_Generator.NPC_Classes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NPC_Generator
{
    public class NpcManager 
    {
        [SerializeField] internal string m_npcName;
        [SerializeField] public List<NPC_Human> allNPCs = new List<NPC_Human>();
        public static int BasicHumanHash;
        public static Dictionary<string, GameObject> HumanPreset = new Dictionary<string, GameObject>();
        public static Dictionary<string, string[]> ArmorSets = new Dictionary<string, string[]>
        {
            {"Troll",new string[]{"HelmetTrollLeather","CapeTrollHide","ArmorTrollLeatherChest","ArmorTrollLeatherLegs"}},
            {"Troll0",new string[]{"CapeTrollHide","ArmorTrollLeatherChest","ArmorTrollLeatherLegs"}},
            {"Bronze",new string[]{"ArmorBronzeChest","ArmorBronzeLegs","HelmetBronze","CapeTrollHide"}},
            {"Iron",new string[]{"ArmorIronChest","ArmorIronLegs","HelmetIron","CapeLinen"}},
            {"Silver",new string[]{"ArmorWolfChest","ArmorWolfLegs","HelmetDrake","CapeWolf"}},
            {"Padded",new string[]{"ArmorPaddedCuirass","ArmorPaddedGreaves","HelmetPadded","CapeLinen"}},
            {"Padded0",new string[]{"ArmorPaddedCuirass","ArmorPaddedGreaves","CapeLinen"}}
        };
        public static string[] Weapons = { "AtgeirBlackmetal", "AtgeirBronze", "AtgeirIron", "Battleaxe", "KnifeBlackMetal", "KnifeChitin", "KnifeCopper", "KnifeFlint", "MaceBronze", "MaceIron", "MaceNeedle", "MaceSilve",
		 "SledgeIron", "SledgeStagbreaker", "SpearBronze", "SpearElderbark", "SpearFlint", "SpearWolfFang", "SwordBlackmetal", "SwordBronze","SwordIron", "SwordSilver", "AtgeirBlackmetal",
		 "AtgeirBronze", "AtgeirIron", "Battleaxe", "KnifeBlackMetal", "KnifeChitin", "KnifeCopper", "KnifeFlint", "MaceBronze", "MaceIron", "MaceNeedle", "MaceSilver" };
		
        public static string[] Armor = { "ArmorBronzeChest", "ArmorBronzeLegs", "ArmorIronChest", "ArmorIronLegs", "ArmorLeatherChest", "ArmorLeatherLegs", "ArmorPaddedCuirass", "ArmorPaddedGreaves", "ArmorRagsChest",
		 "ArmorRagsLegs", "ArmorTrollLeatherChest", "ArmorTrollLeatherLegs", "ArmorWolfChest", "ArmorWolfLegs", "CapeDeerHide", "CapeLinen", "CapeLox", "CapeTrollHide", "CapeWolf", "HelmetBronze", "HelmetDrake",
		 "HelmetIron", "HelmetLeather", "HelmetPadded", "HelmetTrollLeather", "HelmetYule" };
		
        public static string[] Shield = { "ShieldBanded", "ShieldBlackmetal", "ShieldBlackmetalTower", "ShieldBronzeBuckler", "ShieldIronSquare", "ShieldIronTower", "ShieldKnight", "ShieldSerpentscale", "ShieldSilver", "ShieldWood", "ShieldWoodTower" };
		
        public static string[] Tools = { "AxeIron", "PickAxeIron" };
		private class humanData
		{
			public string presetNAME = "MidEnemy1";
			public string prefab = "Goblin";
			public bool isFriend = false;
			public float m_randomMoveInterval = 30;
			public float m_randomMoveRange = 3;
			public float m_moveMinAngle = 30;
			public float health = 200;
			public float speed = 7;
			public string sets = "Troll";
			public string[] weapons = { "SwordBronze", "SwordIron", "AtgeirBronze", "AtgeirIron", "SpearBronze" };
			public string[] sheild = { "ShieldBanded", "ShieldBlackmetal", "ShieldBlackmetalTower", "ShieldBronzeBuckler", "ShieldIronSquare", "ShieldIronTower", "ShieldKnight", "ShieldSerpentscale", "ShieldSilver", "ShieldWood", "ShieldWoodTower" };
			//public string[] armor = { "ArmorBronzeChest", "ArmorBronzeLegs", "ArmorIronChest", "ArmorIronLegs", "CapeTrollHide", "CapeWolf", "HelmetBronze", "HelmetDrake", "HelmetIron" };
		}
		private static List<humanData> presets = new List<humanData>
		{
			new humanData(),
			new humanData(){ 
				presetNAME="LowEnemey1",health=300,
			weapons = new string[]{"Club","SpearFlint","KnifeFlint"},
			},
			new humanData(){
				presetNAME="Fighter1",health=500,
			sets="Troll0",
			weapons = new string[]{"Club","SpearFlint","KnifeFlint"},
			},
			new humanData(){
				presetNAME="Fighter2",health=500,
			sets="Brozen",
			},
			new humanData(){
				presetNAME="DumbNPC",health=300,
			sets="Troll0",
			weapons = new string[]{"SwordBronze", "SwordIron"},
			m_randomMoveRange=0,
			isFriend=true,
			},
			new humanData(){
				presetNAME="DumbWorker",health=300,
			sets="Troll0",
			m_randomMoveRange=0,
			weapons=Tools,
			sheild=new string[]{""},
			isFriend=true,
			},
			new humanData(){
				presetNAME="GuardNPC",health=300,
			sets="Padded0",
			m_randomMoveInterval=5,
			m_randomMoveRange=60,
			isFriend=true,
			}
		};
		

        internal static void SetupNPC()
        {
	        var go = Object.Instantiate(Game.instance.m_playerPrefab, NPC_Generator.RootGOHolder.transform);
	        Object.DestroyImmediate(go.GetComponent<PlayerController>());
	        Object.DestroyImmediate(go.GetComponent<Player>());
	        Object.DestroyImmediate(go.GetComponent<Talker>());
	        Object.DestroyImmediate(go.GetComponent<Skills>());
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
	        NPC_Generator.NetworkedNPC = go;
	        NPC_Generator.NetworkedNPC.name = "BasicHuman";
        }
        
        internal static void Init()
        {
	        var go = NPC_Generator.NetworkedNPC;
	        var MonsterAI =
		        go.AddComponent<MonsterAI>();
	        SetupMonsterAI(MonsterAI);
	        MonsterAI.m_nview = go.GetComponent<ZNetView>();
	        MonsterAI.m_nview.m_zdo = new ZDO();
	        var hum = go.GetComponent<Humanoid>();
	        hum.m_faction = Character.Faction.PlainsMonsters;
	        hum.m_health = 200;
	        hum.m_defaultItems = new GameObject[0];
	        NPC_Generator.NetworkedNPC = go;
        }

        internal static void ZNSStuff()
        {
	        var go = NPC_Generator.NetworkedNPC;
	        var hum = go.GetComponent<Humanoid>();
	        hum.m_randomSets = new Humanoid.ItemSet[1] { NpcManager.GetSet("Iron") };
	        hum.m_unarmedWeapon = null;
	        hum.m_randomWeapon = NpcManager.RandomVis(NpcManager.Weapons);
	        hum.m_randomShield = NpcManager.RandomVis(NpcManager.Shield);
	        NPC_Generator.NetworkedNPC = go;
        }
        

    internal static void SetupMonsterAI(MonsterAI ai)
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

        public static GameObject[] RandomVis(string[] list)
        {
            if (list.Length == 0)
            {
                return new GameObject[0];
            }
            GameObject[] items = new GameObject[list.Length];
            int i = 0;
            foreach (var item in list)
            {
                items[i] = ZNetScene.instance.GetPrefab(item);
                i++;
            }

            return items;
        }
        public static Humanoid.ItemSet GetSet(string set_name)
        {
            Humanoid.ItemSet result = new Humanoid.ItemSet();
            string[] list = ArmorSets[set_name];
            result.m_name = set_name;
            var sets = RandomVis(list);
            result.m_items = sets;
            return result;
        }
        
    }
}