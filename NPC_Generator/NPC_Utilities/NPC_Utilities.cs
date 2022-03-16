using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NPC_Generator.Libs;
using NPC_Generator.MonoScripts;
using NPC_Generator.Tools;
using UnityEngine;
using static NPC_Generator.NPC_Generator;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace NPC_Generator.NPC_Utilities
{
	public static class NpcUtilities
    {
	    #region HelperFuncs
	    public static int seed = 0;
	    private static T? CopyChildrenComponents<T, TU>(this Component comp, TU other) where T : Component
		{
			IEnumerable<FieldInfo> finfos = comp.GetType().GetFields(BindingFlags);
			foreach (var finfo in finfos)
			{
				finfo.SetValue(comp, finfo.GetValue(other));
			}
			return comp as T;
		}
		private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField;
		private static T? GetCopyOf<T>(this Component comp, T other) where T : Component
		{
			Type type = comp.GetType();
			if (type != other.GetType()) return null; // type mis-match

			List<Type> derivedTypes = new List<Type>();
			Type derived = type.BaseType;
			while (derived != null)
			{
				if (derived == typeof(MonoBehaviour))
				{
					break;
				}
				derivedTypes.Add(derived);
				derived = derived.BaseType;
			}

			IEnumerable<PropertyInfo> pinfos = type.GetProperties(BindingFlags);

			foreach (Type derivedType in derivedTypes)
			{
				pinfos = pinfos.Concat(derivedType.GetProperties(BindingFlags));
			}

			pinfos = from property in pinfos
					 where !(type == typeof(Rigidbody) && property.Name == "inertiaTensor") // Special case for Rigidbodies inertiaTensor which isn't catched for some reason.
					 where !property.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(ObsoleteAttribute))
					 select property;
			foreach (var pinfo in pinfos)
			{
				if (pinfo.CanWrite)
				{
					if (pinfos.Any(e => e.Name == $"shared{char.ToUpper(pinfo.Name[0])}{pinfo.Name.Substring(1)}"))
					{
						continue;
					}
					try
					{
						pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
					}
					catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
				}
			}

			IEnumerable<FieldInfo> finfos = type.GetFields(BindingFlags);

			foreach (var finfo in finfos)
			{

				foreach (Type derivedType in derivedTypes)
				{
					if (finfos.Any(e => e.Name == $"shared{char.ToUpper(finfo.Name[0])}{finfo.Name.Substring(1)}"))
					{
						continue;
					}
					finfos = finfos.Concat(derivedType.GetFields(BindingFlags));
				}
			}

			foreach (var finfo in finfos)
			{
				finfo.SetValue(comp, finfo.GetValue(other));
			}

			finfos = from field in finfos
					 where field.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(ObsoleteAttribute))
					 select field;
			foreach (var finfo in finfos)
			{
				finfo.SetValue(comp, finfo.GetValue(other));
			}

			return comp as T;
		}
		public static T? AddComponentcc<T>(this GameObject go, T toAdd) where T : Component
		{
			return go.AddComponent(toAdd.GetType()).GetCopyOf(toAdd) as T;
		}
		public static float RollDice(this float val)
		{
			seed += 1;
			UnityEngine.Random.InitState((int)((Time.time + val) * 1000) + seed);
			return val * UnityEngine.Random.value;
		}
		public static int RollDice(this int val)
		{
			seed += 1;
			UnityEngine.Random.InitState((int)((Time.time + val) * 1000) + seed);
			return Mathf.FloorToInt(UnityEngine.Random.Range(0, val - 0.0001f));
		}
		public static string GetRandomElement(this string[] array)
		{
			return array[array.Length.RollDice()];
		}
		public static ItemDrop.ItemData GetItemData(string name)
		{
			return ObjectDB.instance.GetItemPrefab(name).GetComponent<ItemDrop>().m_itemData;
		}
		#endregion HelperFuncs

		#region NPCMethods

		/// <summary>
		/// Called to build human male NPC base prefab that all other male NPC's are cloned from
		/// </summary>
		internal static void BuildMaleHumanNpc()
        {
            var temp = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject? tempplayer = null;
            foreach (var o in temp)
            {
                if (o.GetComponent<Player>() != null)
                { 
	                NetworkedNPCMale = Object.Instantiate(o, RootGOHolder?.transform!);
                    tempplayer = o;
                    if (!BadgerPlayerMeshMod)
                    {
	                    break;
                    }
                }
                if (BadgerPlayerMeshMod)
                {
	                if (o.name == "Male")
	                {
		                if (o.transform.Find("Body") != null)
		                {
			               
			                playerMale = o;
			                continue; 
		                }
	                }
                }
            }
            if (BadgerPlayerMeshMod)
            {
	            foreach (var VARIABLE in Resources.FindObjectsOfTypeAll<Texture2D>())
	            {
		            if (VARIABLE.name == "noMetal")
		            {
			            noMetal = VARIABLE;
		            }
	            }
            }
            if (BadgerPlayerMeshMod)
            {
	            var player = NetworkedNPCMale!.GetComponent<Player>();
	            DebugLog(DebugLevel.All,"Got instance");
	            VisEquipment component = NetworkedNPCMale.GetComponent<VisEquipment>();
	            DebugLog(DebugLevel.All,"got ve");
	            maleSmr = playerMale!.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
	            DebugLog(DebugLevel.All,"got renderers");
	            maleSkin = maleSmr.sharedMaterial.GetTexture("_MainTex");
	            maleSkinBump = maleSmr.sharedMaterial.GetTexture("_BumpMap");
	             DebugLog(DebugLevel.All,"Got materials");
	            if (component != null)
	            {
		            component.m_models[0].m_mesh = maleSmr.sharedMesh;
		            component.m_models[0].m_baseMaterial.SetTexture("_MainTex", maleSkin);
		            component.m_models[0].m_baseMaterial.SetTexture("_SkinBumpMap", maleSkinBump);
	            }
	            UpdateBody.UpdateBodyModel(component!);
            }
            Object.DestroyImmediate(NetworkedNPCMale?.GetComponent<PlayerController>());
            Object.DestroyImmediate(NetworkedNPCMale?.GetComponent<Player>());
            Object.DestroyImmediate(NetworkedNPCMale?.GetComponent<Talker>());
            Object.DestroyImmediate(NetworkedNPCMale?.GetComponent<Skills>());
            NetworkedNPCMale!.name = "BasicHumanMale";
            var basicznet =
	            NetworkedNPCMale.GetComponent<ZNetView>();
            basicznet.enabled = true;
            NetworkedNPCMale.GetComponent<ZSyncAnimation>().enabled = true;
            NetworkedNPCMale.GetComponent<ZSyncTransform>().enabled = true;
            var HumanoidAI = NetworkedNPCMale.AddComponent<Humanoid>();
            HumanoidAI.m_nview = basicznet;
            basicznet.m_persistent = true;
            HumanoidAI.CopyChildrenComponents<Humanoid, Player>(tempplayer!.GetComponent<Player>());
            NetworkedNPCMale.name = "BasicHumanMale";
            NetworkedNPCMale.transform.name = "BasicHumanMale";
            NetworkedNPCMale.transform.position = Vector3.zero;
            var MonsterAI =
	            NetworkedNPCMale.AddComponent<MonsterAI>();
            SetupMonsterAI(MonsterAI);
            MonsterAI.m_nview = NetworkedNPCMale.GetComponent<ZNetView>();
            MonsterAI.m_nview.m_zdo = new ZDO();
            var hum = NetworkedNPCMale.GetComponent<Humanoid>();
            hum.m_faction = Character.Faction.PlainsMonsters;
            hum.m_health = 200;
            hum.m_defaultItems = new GameObject[0];
            hum.m_eye = NetworkedNPCMale.transform.Find("EyePos");
            hum.m_deathEffects = tempplayer.GetComponent<Player>().m_deathEffects;
        }
		
		/// <summary>
		/// Called to initialize the base human female prefab that will be used to build NPC's upon
		/// </summary>
		internal static void BuildFemaleHumanNpc()
        {
            var temp = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject? tempplayer = null;
            foreach (var o in temp)
            {
                if (o.GetComponent<Player>() != null)
                { 
	                NetworkedNPCFemale = Object.Instantiate(o, RootGOHolder?.transform!);
                    tempplayer = o;
                    if (!BadgerPlayerMeshMod)
                    {
	                    break;
                    }
                }
                if (BadgerPlayerMeshMod)
                {
	                if (o.name == "Female")
	                {
		                if(o.GetComponentInChildren<SkinnedMeshRenderer>() != null)
		                {
			                playerFemale = o;
			                continue;
		                }
	                }
                }
            }
            if (BadgerPlayerMeshMod)
            {
	            foreach (var VARIABLE in Resources.FindObjectsOfTypeAll<Texture2D>())
	            {
		            if (VARIABLE.name == "noMetal")
		            {
			            noMetal = VARIABLE;
		            }
	            }
            }
            if (BadgerPlayerMeshMod)
            {
	            var player = NetworkedNPCFemale!.GetComponent<Player>();
	            DebugLog(DebugLevel.All,"Got instance");
	            VisEquipment component = NetworkedNPCFemale!.GetComponent<VisEquipment>();
	            DebugLog(DebugLevel.All,"got ve");
	            femaleSmr = playerFemale!.GetComponentInChildren<SkinnedMeshRenderer>();
	            DebugLog(DebugLevel.All,"got renderers");
	            femaleSkin = femaleSmr!.sharedMaterial.GetTexture("_MainTex");
	            femaleSkinBump = femaleSmr!.sharedMaterial.GetTexture("_BumpMap");
	            DebugLog(DebugLevel.All,"Got materials");
	            if (component != null)
	            {
		            component.m_models[1].m_mesh = femaleSmr?.sharedMesh;
		            component.m_models[1].m_baseMaterial.SetTexture("_MainTex", femaleSkin);
		            component.m_models[1].m_baseMaterial.SetTexture("_SkinBumpMap", femaleSkinBump);
	            }
	            UpdateBody.UpdateBodyModel(component!);
            }
			var basicznet =
				NetworkedNPCFemale!.GetComponent<ZNetView>();
            Object.DestroyImmediate(NetworkedNPCFemale?.GetComponent<PlayerController>());
            Object.DestroyImmediate(NetworkedNPCFemale?.GetComponent<Player>());
            Object.DestroyImmediate(NetworkedNPCFemale?.GetComponent<Talker>());
            Object.DestroyImmediate(NetworkedNPCFemale?.GetComponent<Skills>());
            NetworkedNPCFemale!.name = "BasicHumanFemale";
            NetworkedNPCFemale.AddComponent<FemaleAssigner>();
            basicznet.enabled = true;
            NetworkedNPCFemale.GetComponent<ZSyncAnimation>().enabled = true;
            NetworkedNPCFemale.GetComponent<ZSyncTransform>().enabled = true;
            var HumanoidAI = NetworkedNPCFemale.AddComponent<Humanoid>();
            HumanoidAI.m_nview = basicznet;
            basicznet.m_persistent = true;
            HumanoidAI.CopyChildrenComponents<Humanoid, Player>(tempplayer!.GetComponent<Player>());
            NetworkedNPCFemale.name = "BasicHumanFemale";
            NetworkedNPCFemale.transform.name = "BasicHumanFemale";
            NetworkedNPCFemale.transform.position = Vector3.zero;
            var MonsterAI =
	            NetworkedNPCFemale.AddComponent<MonsterAI>();
            SetupMonsterAI(MonsterAI);
            MonsterAI.m_nview = NetworkedNPCFemale.GetComponent<ZNetView>();
            MonsterAI.m_nview.m_zdo = new ZDO();
            var hum = NetworkedNPCFemale.GetComponent<Humanoid>();
            hum.m_faction = Character.Faction.PlainsMonsters;
            hum.m_health = 200;
            hum.m_defaultItems = new GameObject[0];
            hum.m_eye = NetworkedNPCFemale.transform.Find("EyePos");
            hum.m_deathEffects = tempplayer.GetComponent<Player>().m_deathEffects;
        }

		internal static void BuildRaidNPC()
		{
			
		var temp = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject? tempplayer = null;
            foreach (var o in temp)
            {
                if (o.GetComponent<Player>() != null)
                { 
	                NetworkRaider = Object.Instantiate(o, RootGOHolder?.transform!);
                    tempplayer = o;
                    if (!BadgerPlayerMeshMod)
                    {
	                    break;
                    }
                }
                if (BadgerPlayerMeshMod)
                {
	                if (o.name == "Male")
	                {
		                if (o.transform.Find("Body") != null)
		                {
			               
			                playerMale = o;
			                continue; 
		                }
	                }
                }
            }
            if (BadgerPlayerMeshMod)
            {
	            foreach (var VARIABLE in Resources.FindObjectsOfTypeAll<Texture2D>())
	            {
		            if (VARIABLE.name == "noMetal")
		            {
			            noMetal = VARIABLE;
		            }
	            }
            }
            if (BadgerPlayerMeshMod)
            {
	            var player = NetworkRaider!.GetComponent<Player>();
	            DebugLog(DebugLevel.All,"Got instance");
	            VisEquipment component = NetworkRaider.GetComponent<VisEquipment>();
	            DebugLog(DebugLevel.All,"got ve");
	            maleSmr = playerMale!.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
	            DebugLog(DebugLevel.All,"got renderers");
	            maleSkin = maleSmr.sharedMaterial.GetTexture("_MainTex");
	            maleSkinBump = maleSmr.sharedMaterial.GetTexture("_BumpMap");
	             DebugLog(DebugLevel.All,"Got materials");
	            if (component != null)
	            {
		            component.m_models[0].m_mesh = maleSmr.sharedMesh;
		            component.m_models[0].m_baseMaterial.SetTexture("_MainTex", maleSkin);
		            component.m_models[0].m_baseMaterial.SetTexture("_SkinBumpMap", maleSkinBump);
	            }
	            UpdateBody.UpdateBodyModel(component!);
            }
            Object.DestroyImmediate(NetworkRaider?.GetComponent<PlayerController>());
            Object.DestroyImmediate(NetworkRaider?.GetComponent<Player>());
            Object.DestroyImmediate(NetworkRaider?.GetComponent<Talker>());
            Object.DestroyImmediate(NetworkRaider?.GetComponent<Skills>());
            NetworkRaider!.name = "VillageRaider";
            var basicznet =
	            NetworkedNPCMale?.GetComponent<ZNetView>();
            basicznet!.enabled = true;
            NetworkRaider.GetComponent<ZSyncAnimation>().enabled = true;
            NetworkRaider.GetComponent<ZSyncTransform>().enabled = true;
            var HumanoidAI = NetworkRaider.AddComponent<Humanoid>();
            HumanoidAI.m_nview = basicznet;
            basicznet.m_persistent = true;
            HumanoidAI.CopyChildrenComponents<Humanoid, Player>(tempplayer!.GetComponent<Player>());
            NetworkRaider.name = "VillageRaider";
            NetworkRaider.transform.name = "VillageRaider";
            NetworkRaider.transform.position = Vector3.zero;
            var MonsterAI =
	            NetworkRaider.AddComponent<MonsterAI>();
            SetupMonsterAI(MonsterAI);
            MonsterAI.m_nview = NetworkRaider.GetComponent<ZNetView>();
            MonsterAI.m_nview.m_zdo = new ZDO();
            var hum = NetworkRaider.GetComponent<Humanoid>();
            hum.m_faction = Character.Faction.PlainsMonsters;
            hum.m_health = 200;
            hum.m_defaultItems = new GameObject[0];
            hum.m_eye = NetworkRaider.transform.Find("EyePos");
            hum.m_deathEffects = tempplayer.GetComponent<Player>().m_deathEffects;
            NetworkRaider.AddComponent<RandomVisuals>();
            NetworkRaider.AddComponent<RandomWeapon>();
            NetworkRaider.AddComponent<CapsuleHelper>();
		}
		
		/// <summary>
		/// Configure's base MonsterAI
		/// </summary>
		/// <param name="ai"></param>
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
		
		/// <summary>
		/// Setup Monster AI from config
		/// </summary>
		/// <param name="ai"></param>
		/// <param name="monsterAIConfig"></param>
        internal static void SetupMonsterAI(MonsterAI ai, MonsterAIConfig monsterAIConfig)
        {
            ai.m_viewRange = monsterAIConfig.mViewRange;
            ai.m_viewAngle = monsterAIConfig.mViewAngle;
            ai.m_hearRange = monsterAIConfig.mHearRange;
            ai.m_alertedEffects.m_effectPrefabs = new EffectList.EffectData[0];
            ai.m_idleSound.m_effectPrefabs = new EffectList.EffectData[0];
            ai.m_pathAgentType = Pathfinding.AgentType.Humanoid;
            ai.m_smoothMovement = true;
            ai.m_jumpInterval = 20.RollDice();
            ai.m_randomCircleInterval = monsterAIConfig.mCircleInteverval;
            ai.m_circleTargetDistance = monsterAIConfig.mCircleDistance;
            ai.m_circleTargetDuration = monsterAIConfig.mCircleDuration;
            ai.m_circulateWhileCharging = monsterAIConfig.mCirculateCharge;
            ai.m_randomMoveInterval = monsterAIConfig.npcRandomMoveInterval;
            ai.m_randomMoveRange = monsterAIConfig.npcRandomMoveRange;
            ai.m_alertRange = monsterAIConfig.mAlertRange;
            ai.m_fleeIfHurtWhenTargetCantBeReached = monsterAIConfig.mFleeifHurt;
            ai.m_fleeIfLowHealth = monsterAIConfig.mFleeLowHealth;
            ai.m_circulateWhileCharging = monsterAIConfig.mCirculateCharge;
            ai.m_fleeIfNotAlerted = monsterAIConfig.mFleeNotAlert;
            ai.m_enableHuntPlayer = monsterAIConfig.npcHuntPlayer;
            ai.m_wakeupRange = 5;
            ai.m_wakeupEffects.m_effectPrefabs = new EffectList.EffectData[0];
            ai.m_interceptTimeMax = monsterAIConfig.mInterceptMax;
            ai.m_interceptTimeMin = monsterAIConfig.mInterceptMin;
            ai.m_attackPlayerObjects = monsterAIConfig.mAttackPlayerObj;
            ai.m_maxChaseDistance = monsterAIConfig.mChaseDistance;
            ai.m_minAttackInterval = monsterAIConfig.mAttackInterval;
            ai.m_avoidWater = monsterAIConfig.npcAvoidWater;
            ai.m_avoidLand = monsterAIConfig.npcAvoidLand;
            ai.m_avoidFire = monsterAIConfig.npcAvoidFire;
        }

		#endregion
    }
}