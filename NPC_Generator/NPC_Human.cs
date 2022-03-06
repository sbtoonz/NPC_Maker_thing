using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace NPC_Generator
{
    public static class NPC_Human
    {
        private static List<ItemDrop> m_ChestGear = new List<ItemDrop>();
        private static  List<ItemDrop> m_Helmet = new List<ItemDrop>();
        private static List<ItemDrop> m_Shoulder = new List<ItemDrop>();
        private static List<ItemDrop> m_weapons = new List<ItemDrop>();
        private static List<ItemDrop> m_pants = new List<ItemDrop>();
        private static  List<ItemDrop> m_Shields = new List<ItemDrop>();
        private static Humanoid? _humanoid;
        private static List<GameObject> spawnedNPCs = new List<GameObject>();
        internal static void SetupArmor(GameObject go)
        {
            _humanoid = go.GetComponent<Humanoid>();
            if(ObjectDB.instance.GetItemPrefab("Wood")==null) return;
            PopulateChestGear();
            PopulateHelmets();
            PopulateShoulder();
            PopulateWeapons();
            PopulatePants();
            PopulateShield();
            CreateSetLists();
        }

        private static void PopulateChestGear()
        {
            m_ChestGear = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.Chest, "Armor");
        }
        private static void PopulateHelmets()
        {
            m_Helmet = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.Helmet, "Helmet");
        }
        private static void PopulateShoulder()
        {
            m_Shoulder = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.Shoulder, "Cape");
        }
        private static void PopulateWeapons()
        {
            m_weapons = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.OneHandedWeapon, "Axe");
            m_weapons.AddRange(ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.TwoHandedWeapon, "Battleaxe"));
        }
        private static void PopulatePants()
        {
            m_pants = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.Legs, "Armor");
        }
        private static void PopulateShield()
        {
            m_Shields = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.Shield, "Shield");
        }
        private static void CreateSetLists()
        {
            _humanoid!.m_defaultItems = new GameObject[0];
            string helmet = m_Helmet[Random.Range(0, m_Helmet.Count)].name;
            string chest = m_ChestGear[Random.Range(0, m_ChestGear.Count)].name;
            string shoulder = m_Shoulder[Random.Range(0, m_Shoulder.Count)].name;
            string pants = m_pants[Random.Range(0, m_pants.Count)].name;
            _humanoid.m_randomSets = new Humanoid.ItemSet[1]
            {
                new Humanoid.ItemSet
                {
                    m_items = new []
                    {
                        ZNetScene.instance.GetPrefab(helmet),
                        ZNetScene.instance.GetPrefab(chest), 
                        ZNetScene.instance.GetPrefab(shoulder),
                        ZNetScene.instance.GetPrefab(pants)
                    }
                }
            };
            _humanoid.m_randomWeapon = new []
            {
                ZNetScene.instance.GetPrefab(m_weapons[Random.Range(0, m_weapons.Count)].name)
            };
            _humanoid.m_randomShield = new[]
            {
                ZNetScene.instance.GetPrefab(m_Shields[Random.Range(0, m_Shields.Count)].name)
            };
            ;
        }

        private static Humanoid.ItemSet[] CreateSetList(NPCYamlConfig npcYamlConfig, ZNetScene netScene)
        {
            var set = new Humanoid.ItemSet[1]
            {
                new Humanoid.ItemSet
                {
                    m_items = new []
                    {
                        netScene.GetPrefab(npcYamlConfig.npcHelmetString),
                        netScene.GetPrefab(npcYamlConfig.npcChestString), 
                        netScene.GetPrefab(npcYamlConfig.npcShoulder),
                        netScene.GetPrefab(npcYamlConfig.npcLegString)
                    }
                }
            };
            return set;
        }

        private static void SetupVisuals(Humanoid humanoid, NPCYamlConfig config, ZNetScene netScene)
        {
            humanoid.m_randomSets = CreateSetList(config, netScene);
            if (!config.npcWeapon.IsNullOrWhiteSpace())
            {
                humanoid.m_randomWeapon = new []
                {
                    netScene.GetPrefab(config.npcWeapon)
                }; 
            }
            else
            {
                humanoid.m_randomWeapon = Array.Empty<GameObject>();
            }

            if (!config.npcShield.IsNullOrWhiteSpace())
            {
                humanoid.m_randomShield = new[]
                {
                    netScene.GetPrefab(config.npcShield)
                };
            }
            else
            {
                humanoid.m_randomShield = Array.Empty<GameObject>();
            }

            humanoid.m_faction = config.npcFaction switch
            {
                "Player" => Character.Faction.Players,
                "AnimalsVeg" => Character.Faction.AnimalsVeg,
                "ForestMonsters" => Character.Faction.ForestMonsters,
                "Undead" => Character.Faction.Undead,
                "Demon" => Character.Faction.Demon,
                "MountainMonsters" => Character.Faction.MountainMonsters,
                "SeaMonsers" => Character.Faction.SeaMonsters,
                "PlainsMonsters" => Character.Faction.PlainsMonsters,
                _ => humanoid.m_faction
            };
        }

        internal static GameObject ReturnNamedNpc(string npcName, NPCYamlConfig config, ZNetScene scene)
        {
            if (spawnedNPCs.Contains(scene.GetPrefab(npcName)))
            {
                var temp = spawnedNPCs.Find(x => x.name == npcName);
                spawnedNPCs.Remove(temp);
                Object.Destroy(temp);
            }
            var go = Object.Instantiate(NPC_Generator.NetworkedNPC, NPC_Generator.RootGOHolder!.transform);
            go!.name = npcName.Replace(" ", String.Empty);
            SetupVisuals(go.GetComponent<Humanoid>(), config, scene);
            var mai = go.GetComponent<MonsterAI>();
            mai.m_enableHuntPlayer = config.npcHuntPlayer;
            mai.m_huntPlayer = config.npcHuntPlayer;
            go.transform.localScale = new Vector3(config.npcScale, config.npcScale, config.npcScale);
            go.GetComponent<ZNetView>().m_syncInitialScale = true;
            spawnedNPCs.Add(go);
            return go;
        }
        
    }
}