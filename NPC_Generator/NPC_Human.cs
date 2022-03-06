using System;
using System.Collections.Generic;
using BepInEx;
using NPC_Generator.MonoScripts;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NPC_Generator
{
    public static class NPC_Human
    {
        private static List<GameObject> spawnedNPCs = new List<GameObject>();
        private static Humanoid.ItemSet[] CreateSetList(NPCYamlConfig npcYamlConfig, ZNetScene netScene)
        {
            GameObject? helmet = null;
            GameObject? chest = null;
            GameObject? shoulder = null;
            GameObject? leg = null;
            List<GameObject> gameObjects = new List<GameObject>();
            if (!npcYamlConfig.npcHelmetString.IsNullOrWhiteSpace())
            {
                helmet = netScene.GetPrefab(npcYamlConfig.npcHelmetString);
                gameObjects.Add(helmet);
            }

            if (!npcYamlConfig.npcChestString.IsNullOrWhiteSpace())
            {
                chest = netScene.GetPrefab(npcYamlConfig.npcChestString);
                gameObjects.Add(chest);
            }

            if (!npcYamlConfig.npcShoulder.IsNullOrWhiteSpace())
            {
                shoulder = netScene.GetPrefab(npcYamlConfig.npcShoulder);
                gameObjects.Add(shoulder);
            }

            if (!npcYamlConfig.npcLegString.IsNullOrWhiteSpace())
            {
                leg = netScene.GetPrefab(npcYamlConfig.npcLegString);
                gameObjects.Add(leg);
            }
            var set = new Humanoid.ItemSet[1]
            {
                new Humanoid.ItemSet
                {
                    m_items = gameObjects.ToArray()
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
            else if(config.npcWeapon.IsNullOrWhiteSpace())
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
            else if(config.npcShield.IsNullOrWhiteSpace())
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

            GameObject tempNPC = null;
            switch (config.npcSex.ToLower())
            {
                case "male":
                {
                    tempNPC = Object.Instantiate(NPC_Generator.NetworkedNPCMale, NPC_Generator.RootGOHolder!.transform);
                    tempNPC!.name = npcName.Replace(" ", String.Empty);
                    SetupVisuals(tempNPC.GetComponent<Humanoid>(), config, scene);
                    var mai = tempNPC.GetComponent<MonsterAI>();
                    mai.m_enableHuntPlayer = config.npcHuntPlayer;
                    mai.m_huntPlayer = config.npcHuntPlayer;
                    tempNPC.transform.localScale = new Vector3(config.npcScale, config.npcScale, config.npcScale);
                    tempNPC.GetComponent<ZNetView>().m_syncInitialScale = true;
                    if (config.npcTameable)
                    {
                        tempNPC.AddComponent<Tameable>();
                        tempNPC.AddComponent<TameHelper>();
                    }
                    var hair =tempNPC.AddComponent<HairSetter>();
                    hair.HairStyleName = config.npcHairStyle;
                    spawnedNPCs.Add(tempNPC);
                    return tempNPC;
                }
                case "female":
                {
                    tempNPC = Object.Instantiate(NPC_Generator.NetworkedNPCFemale, NPC_Generator.RootGOHolder!.transform);
                    tempNPC!.name = npcName.Replace(" ", String.Empty);
                    SetupVisuals(tempNPC.GetComponent<Humanoid>(), config, scene);
                    var mai = tempNPC.GetComponent<MonsterAI>();
                    mai.m_enableHuntPlayer = config.npcHuntPlayer;
                    mai.m_huntPlayer = config.npcHuntPlayer;
                    tempNPC.transform.localScale = new Vector3(config.npcScale, config.npcScale, config.npcScale);
                    tempNPC.GetComponent<ZNetView>().m_syncInitialScale = true;
                    if (config.npcTameable)
                    {
                        tempNPC.AddComponent<Tameable>();
                        tempNPC.AddComponent<TameHelper>();
                    }
                    var hair =tempNPC.AddComponent<HairSetter>();
                    hair.HairStyleName = config.npcHairStyle;
                    spawnedNPCs.Add(tempNPC);
                    return tempNPC;
                }
                default:
                    return tempNPC;
            }
        }
        
    }
}