using System.Collections.Generic;
using NPC_Generator.NPC_Classes;
using UnityEngine;

namespace NPC_Generator
{
    public class NpcManager : MonoBehaviour
    {
        [SerializeField] internal string m_npcName;
        [SerializeField] public List<NPC_Human> allNPCs = new List<NPC_Human>();
        [SerializeField] public GameObject BasicHuman;
        public static GameObject Root;
        public static GameObject PrefabParent;
        public static NpcManager instance;
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
        private void Awake()
        {
            Root = this.gameObject;
            instance = this;
            PrefabParent = new GameObject("NPCPrefabs");
            PrefabParent.SetActive(false);
            PrefabParent.transform.SetParent(Root.transform);
        }

        internal void SpawnHuman()
        {
            var go = Instantiate(Game.instance.m_playerPrefab, PrefabParent.transform);
            go.transform.position = new Vector3(0, 1000, 0);
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
            var MonsterAI = 
                go.AddComponentcc<MonsterAI>(ZNetScene.instance.GetPrefab("Goblin").GetComponent<MonsterAI>());
            go.AddComponent<Tameable>();
            MonsterAI.m_nview = basicznet;
            go.name = "Basic Human";
            BasicHuman = go;
            ZNetScene.instance.m_prefabs.Add(BasicHuman);
        }

        internal void SpawnNPC()
        {
            var go =Instantiate(BasicHuman);
            go.transform.position = Player.m_localPlayer.transform.position;
            var hum = go.GetComponent<Humanoid>();
            hum.m_randomSets = new Humanoid.ItemSet[1] { GetSet("Troll") };
            var mai = go.GetComponent<MonsterAI>();
            mai.SetAlerted(true);
            hum.m_unarmedWeapon = null;
            hum.m_randomWeapon = RandomVis(Weapons);
            hum.m_randomShield = RandomVis(Shield);
            hum.m_health = 100;
            hum.m_faction = Character.Faction.PlainsMonsters;
            var npchum = go.AddComponent<NPC_Human>();
            allNPCs.Add(npchum);
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