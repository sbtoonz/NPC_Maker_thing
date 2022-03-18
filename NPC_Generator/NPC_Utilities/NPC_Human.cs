using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using NPC_Generator.MonoScripts;
using NPC_Generator.MonoScripts.Villagers;
using NPC_Generator.Tools;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace NPC_Generator.NPC_Utilities
{
    public static class NPC_Human
    {
        private static List<GameObject> spawnedNPCs = new List<GameObject>();
        internal static List<VillagerBase> spawnedVillagers = new List<VillagerBase>();

        /// <summary>
        /// Generates the ItemSet array for the Humanoid component
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <returns></returns>
        private static Humanoid.ItemSet[] CreateSetList(GameObject[] gameObjects)
        {
            var set = new Humanoid.ItemSet[1]
            {
                new Humanoid.ItemSet
                {
                    m_items = gameObjects
                }
            };
            return set;
        }

        private static GameObject[] CreateObjectArray(SetList setList)
        {
            List<GameObject> tempObjects = new List<GameObject>();
            List<GameObject> helmet = new List<GameObject>();
            List<GameObject> chest = new List<GameObject>();
            List<GameObject> shoulder = new List<GameObject>();
            List<GameObject> leg = new List<GameObject>();
            if (setList.npcShoulder.FirstOrDefault()?.ToLower() != "none")
            {
                shoulder.AddRange(setList.npcShoulder.Select(s => ObjectDB.instance.GetItemPrefab(s)));
            }
            if (setList.npcChestString.FirstOrDefault()?.ToLower() != "none")
            {
                chest.AddRange(setList.npcChestString.Select(s=> ObjectDB.instance.GetItemPrefab(s)));
            }
            if (setList.npcHelmetString.FirstOrDefault()?.ToLower() != "none")
            {
                helmet.AddRange(setList.npcHelmetString.Select(s => ObjectDB.instance.GetItemPrefab(s)));
            }
            if (setList.npcLegString.FirstOrDefault()?.ToLower() != "none")
            {
                leg.AddRange(setList.npcLegString.Select(s => ObjectDB.instance.GetItemPrefab(s)));
            }
            foreach (var VARIABLE in helmet)
            {
                tempObjects.Add(VARIABLE);
            }
            foreach (var VARIABLE in chest)
            {
                tempObjects.Add(VARIABLE); 
            }
            foreach (var VARIABLE in shoulder)
            {
                tempObjects.Add(VARIABLE); 
            }
            foreach (var VARIABLE in leg)
            {
                tempObjects.Add(VARIABLE); 
            }
            return tempObjects.ToArray();
        }
        /// <summary>
        /// Sets up the visual equipment lists for the NPC using the YML
        /// </summary>
        /// <param name="humanoid"></param>
        /// <param name="config"></param>
        /// <param name="netScene"></param>
        private static void SetupVisuals(Humanoid humanoid, NPCYamlConfig config, ZNetScene netScene)
        {

            List<Humanoid.ItemSet> setlist = new List<Humanoid.ItemSet>();
            foreach (var setList in config.m_setConfigs)
            {
                var temp = CreateSetList(CreateObjectArray(setList.Value));
                setlist.AddRange(temp);
            }
            humanoid.m_randomSets = setlist.ToArray();
            
            if (config.npcWeapon!.ToArray().GetRandomElement().ToLower() != "none")
            {
                humanoid.m_randomWeapon = new GameObject[] { };
                if(config.npcWeapon.Count > 1)
                {
                    var weaponlist =humanoid.m_randomWeapon.ToList();
                    weaponlist.AddRange(config.npcWeapon.Select(st => ObjectDB.instance.GetItemPrefab(st)));
                    humanoid.m_randomWeapon = weaponlist.ToArray();
                }
                else
                {
                    humanoid.m_randomWeapon = new[]
                    {
                        ObjectDB.instance.GetItemPrefab(config.npcWeapon.ToArray().GetRandomElement())
                    };
                }
            }
            else
            {
                humanoid.m_randomWeapon = Array.Empty<GameObject>();
            }

            if (config.npcShield!.ToArray().GetRandomElement().ToLower() != "none")
            {
                humanoid.m_randomShield = new GameObject[] { };
                if (config.npcShield.Count > 1)
                {
                    var shieldlist = humanoid.m_randomShield.ToList();
                    shieldlist.AddRange(config.npcShield.Select(st => ObjectDB.instance.GetItemPrefab(st)));
                    humanoid.m_randomShield = shieldlist.ToArray();
                }
                else
                {
                    humanoid.m_randomShield = new[]
                    {
                        ObjectDB.instance.GetItemPrefab(config.npcShield.ToArray().GetRandomElement())
                    };
                }
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
                "SeaMonsters" => Character.Faction.SeaMonsters,
                "PlainsMonsters" => Character.Faction.PlainsMonsters,
                _ => humanoid.m_faction
            };
            SetDamageMod(humanoid, config);
            humanoid.m_health = config.mHealth;
            humanoid.m_tolerateFire = config.mTolerateFire;
            humanoid.m_tolerateSmoke = config.mTolerateSmoke;
            humanoid.m_tolerateWater = config.mTolerateWater;
            humanoid.m_tolerateTar = config.mTolerateTar;
            humanoid.m_name = config.mNPCInGameName;
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
            switch (config.npcSex?.ToLower())
            {
                case "male":
                {
                    tempNPC = Object.Instantiate(NPC_Generator.NetworkedNPCMale, NPC_Generator.RootGOHolder!.transform);
                    tempNPC!.name = npcName.Replace(" ", String.Empty);
                    SetupVisuals(tempNPC.GetComponent<Humanoid>(), config, scene);
                    var mai = tempNPC.GetComponent<MonsterAI>();
                    NpcUtilities.SetupMonsterAI(mai, config.monsterAiConfig);
                    var drop = tempNPC.AddComponent<CharacterDrop>();
                    drop.m_drops = createCharDrop(config);
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
                    hair.HairStyleName = config.npcHairStyle!.ToArray().GetRandomElement();
                    hair.hairColor = new Color(config.npcHairColorR, config.npcHairColorG, config.npcHairColorB);
                    if (config.mIsVillager)
                    {
                        tempNPC.AddComponent<VillagerBase>();
                    }
                    if (config.mVillagerBuilder)
                    {
                        Component baseVillagerBase;
                        tempNPC.TryGetComponent(typeof(VillagerBase), out baseVillagerBase);
                        if (baseVillagerBase)
                        {
                            Object.Destroy(baseVillagerBase);
                        }
                        tempNPC.AddComponent<Villager_Builder>();
                    }
                    if (config.mVillagerFarmer)
                    {
                        Component baseVillagerBase;
                        tempNPC.TryGetComponent(typeof(VillagerBase), out baseVillagerBase);
                        if (baseVillagerBase)
                        {
                            Object.Destroy(baseVillagerBase);
                        }  
                        Component BuilderBase;
                        tempNPC.TryGetComponent(typeof(VillagerBase), out BuilderBase);
                        if (BuilderBase)
                        {
                            Object.Destroy(BuilderBase);
                        }
                        tempNPC.AddComponent<Villager_Farmer>();
                    }
                    if (config.mVillagerMessenger)
                    {
                        tempNPC.TryGetComponent(typeof(VillagerBase), out var baseVillagerBase);
                        if (baseVillagerBase)
                        {
                            Object.Destroy(baseVillagerBase);
                        }
                        tempNPC.TryGetComponent(typeof(VillagerBase), out var BuilderBase);
                        if (BuilderBase)
                        {
                            Object.Destroy(BuilderBase);
                        }
                        tempNPC.TryGetComponent(typeof(Villager_Farmer), out var FarmerBase);
                        if (FarmerBase)
                        {
                            Object.Destroy(FarmerBase);
                        }
                        tempNPC.AddComponent<VillagerMessenger>();

                    }
                    if (config.mVillagerRaidMaster)
                    {
                        tempNPC.TryGetComponent(typeof(VillagerBase), out var baseVillagerBase);
                        if (baseVillagerBase)
                        {
                            Object.Destroy(baseVillagerBase);
                        }
                        tempNPC.AddComponent<VillagerRandEvent>();
                    }
                    if (config.mVillagerSkillMaster)
                    {
                        tempNPC.TryGetComponent(typeof(VillagerBase), out var baseVillagerBase);
                        if (baseVillagerBase)
                        {
                            Object.Destroy(baseVillagerBase);
                        }
                        var skill = tempNPC.AddComponent<VillageSkillMaster>();
                        skill.itemNames = config.SkillMastersskills.mItemName.ToArray();
                        skill.skillNames = config.SkillMastersskills.mSkillName.ToArray();
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
                    NpcUtilities.SetupMonsterAI(mai, config.monsterAiConfig);
                    var drop = tempNPC.AddComponent<CharacterDrop>();
                    drop.m_drops = createCharDrop(config);
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
                    hair.HairStyleName = config.npcHairStyle!.ToArray().GetRandomElement();
                    hair.hairColor = new Color(config.npcHairColorR, config.npcHairColorG, config.npcHairColorB);
                    Character character = tempNPC.GetComponent<Character>();
                    spawnedNPCs.Add(tempNPC);
                    if (config.mIsVillager)
                    {
                        tempNPC.AddComponent<VillagerBase>();
                    }
                    if (config.mVillagerBuilder)
                    {
                        Component baseVillagerBase;
                        tempNPC.TryGetComponent(typeof(VillagerBase), out baseVillagerBase);
                        if (baseVillagerBase)
                        {
                            Object.Destroy(baseVillagerBase);
                        }
                        tempNPC.AddComponent<Villager_Builder>();
                    }
                    if (config.mVillagerFarmer)
                    {
                        tempNPC.TryGetComponent(typeof(VillagerBase), out var baseVillagerBase);
                        if (baseVillagerBase)
                        {
                            Object.Destroy(baseVillagerBase);
                        }
                        tempNPC.TryGetComponent(typeof(VillagerBase), out var BuilderBase);
                        if (BuilderBase)
                        {
                            Object.Destroy(BuilderBase);
                        }
                        tempNPC.AddComponent<Villager_Farmer>();
                    }
                    if (config.mVillagerMessenger)
                    {
                        tempNPC.TryGetComponent(typeof(VillagerBase), out var baseVillagerBase);
                        if (baseVillagerBase)
                        {
                            Object.Destroy(baseVillagerBase);
                        }
                        tempNPC.TryGetComponent(typeof(VillagerBase), out var BuilderBase);
                        if (BuilderBase)
                        {
                            Object.Destroy(BuilderBase);
                        }
                        tempNPC.TryGetComponent(typeof(Villager_Farmer), out var FarmerBase);
                        if (FarmerBase)
                        {
                            Object.Destroy(FarmerBase);
                        }
                        tempNPC.AddComponent<VillagerMessenger>();

                    }
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

        internal static List<CharacterDrop.Drop> createCharDrop(NPCYamlConfig config)
        {
            List<CharacterDrop.Drop> m_drops = new List<CharacterDrop.Drop>();
            foreach (var KP in config.DropItems!)
            {
                var drop = returnSingleDrop(KP.Key, KP.Value.m_chance, KP.Value.m_onePer,
                    KP.Value.m_ammountMin,
                    KP.Value.m_ammountMax,
                    KP.Value.m_Multply,
                    ObjectDB.instance);
                m_drops.Add(drop);
            }
            return m_drops;
        }

        internal static CharacterDrop.Drop returnSingleDrop(string DropName,
            float dropChance, 
            bool onePerPlayer,
            int amtMin,
            int amtMax,
            bool multiply,
            ObjectDB objectDB)
        {
            CharacterDrop.Drop newdrop = new CharacterDrop.Drop();
            newdrop.m_prefab = objectDB.GetItemPrefab(DropName);
            newdrop.m_chance = dropChance;
            newdrop.m_amountMax = amtMax;
            newdrop.m_amountMin = amtMin;
            newdrop.m_onePerPlayer = onePerPlayer;
            newdrop.m_levelMultiplier = multiply;
            return newdrop;
        }
        
    }
}