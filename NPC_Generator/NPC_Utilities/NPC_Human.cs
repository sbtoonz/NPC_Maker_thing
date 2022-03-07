using System;
using System.Collections.Generic;
using BepInEx;
using NPC_Generator.MonoScripts;
using NPC_Generator.Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NPC_Generator.NPC_Utilities
{
    public static class NPC_Human
    {
        private static List<GameObject> spawnedNPCs = new List<GameObject>();
        
        /// <summary>
        /// Generates the ItemSet array for the Humanoid component
        /// </summary>
        /// <param name="npcYamlConfig"></param>
        /// <param name="netScene"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Sets up the visual equipment lists for the NPC using the YML
        /// </summary>
        /// <param name="humanoid"></param>
        /// <param name="config"></param>
        /// <param name="netScene"></param>
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
            SetDamageMod(humanoid, config);
            humanoid.m_health = config.mHealth;
        }

        /// <summary>
        /// Called to return an NPC based on the Yml config entry
        /// </summary>
        /// <param name="npcName"></param>
        /// <param name="config"></param>
        /// <param name="scene"></param>
        /// <returns></returns>
        internal static GameObject ReturnNamedNpc(string npcName, NPCYamlConfig config, ZNetScene scene)
        {
            if (spawnedNPCs.Contains(scene.GetPrefab(npcName)))
            {
                var temp = spawnedNPCs.Find(x => x.name == npcName);
                spawnedNPCs.Remove(temp);
                Object.Destroy(temp);
            }

            GameObject? tempNPC = null;
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
                    var skincolor = tempNPC.AddComponent<SkinColorHelper>();
                    skincolor.SkinColor = new Color(config.npcSkinColorR, config.npcSkinColorG, config.npcSkinColorB);
                    hair.HairStyleName = config.npcHairStyle;
                    hair.hairColor = new Color(config.npcHairColorR, config.npcHairColorG, config.npcHairColorB);
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
                    var skincolor = tempNPC.AddComponent<SkinColorHelper>();
                    skincolor.SkinColor = new Color(config.npcSkinColorR, config.npcSkinColorG, config.npcSkinColorB);
                    hair.HairStyleName = config.npcHairStyle;
                    hair.hairColor = new Color(config.npcHairColorR, config.npcHairColorG, config.npcHairColorB);
                    spawnedNPCs.Add(tempNPC);
                    return tempNPC;
                }
                default:
                    return tempNPC!;
            }
        }


        internal static void SetDamageMod(Humanoid humanoid, NPCYamlConfig config)
        {
            humanoid.m_damageModifiers = new HitData.DamageModifiers
            {
                m_blunt = InterpolateConfig(config.DamageResists.mBlunt),
                m_slash = InterpolateConfig(config.DamageResists.mSlash),
                m_pierce = InterpolateConfig(config.DamageResists.mPierce),
                m_chop = InterpolateConfig(config.DamageResists.mChop),
                m_pickaxe = InterpolateConfig(config.DamageResists.mPickaxe),
                m_fire = InterpolateConfig(config.DamageResists.mFire),
                m_frost = InterpolateConfig(config.DamageResists.mFrost),
                m_lightning = InterpolateConfig(config.DamageResists.mLightning),
                m_poison = InterpolateConfig(config.DamageResists.mPoison),
                m_spirit = InterpolateConfig(config.DamageResists.mSpirit)
            };
        }

        private static HitData.DamageModifier InterpolateConfig(string s)
        {
            HitData.DamageModifier modifier = s switch
            {
                "Normal" => HitData.DamageModifier.Normal,
                "Resistant" => HitData.DamageModifier.Resistant,
                "Weak" => HitData.DamageModifier.Weak,
                "Immune" => HitData.DamageModifier.Immune,
                "Ignore" => HitData.DamageModifier.Ignore,
                "Very Resistant" => HitData.DamageModifier.VeryResistant,
                "Very Weak" => HitData.DamageModifier.VeryWeak,
                _ => new HitData.DamageModifier()
            };

            return modifier;
        }
        
    }
}