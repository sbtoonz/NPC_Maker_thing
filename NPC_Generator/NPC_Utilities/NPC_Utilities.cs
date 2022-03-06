using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NPC_Generator.MonoScripts;
using UnityEngine;
using static NPC_Generator.NPC_Generator;
using Object = UnityEngine.Object;

namespace NPC_Generator.NPC_Utilities
{
	public static class NPC_Utilities
    {
	    #region HelperFuncs
		public static T? CopyChildrenComponents<T, TU>(this Component comp, TU other) where T : Component
		{
			IEnumerable<FieldInfo> finfos = comp.GetType().GetFields(bindingFlags);
			foreach (var finfo in finfos)
			{
				finfo.SetValue(comp, finfo.GetValue(other));
			}
			return comp as T;
		}
		private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;
		public static T? GetCopyOf<T>(this Component comp, T other) where T : Component
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

			IEnumerable<PropertyInfo> pinfos = type.GetProperties(bindingFlags);

			foreach (Type derivedType in derivedTypes)
			{
				pinfos = pinfos.Concat(derivedType.GetProperties(bindingFlags));
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

			IEnumerable<FieldInfo> finfos = type.GetFields(bindingFlags);

			foreach (var finfo in finfos)
			{

				foreach (Type derivedType in derivedTypes)
				{
					if (finfos.Any(e => e.Name == $"shared{char.ToUpper(finfo.Name[0])}{finfo.Name.Substring(1)}"))
					{
						continue;
					}
					finfos = finfos.Concat(derivedType.GetFields(bindingFlags));
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
                    break;
                }
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
                    break;
                }
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

		#endregion
    }
}