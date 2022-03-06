using System;
using System.Collections.Generic;
using BepInEx;
using NPC_Generator.MonoScripts;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace NPC_Generator
{
    public static class NPC_Human
    {
        private static List<GameObject> spawnedNPCs = new List<GameObject>();
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
                    spawnedNPCs.Add(tempNPC);
                    return tempNPC;
                }
                default:
                    return tempNPC;
            }
        }
        
    }
}