using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NPC_Generator.MonoScripts;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static NPC_Generator.NPC_Generator;
using Object = UnityEngine.Object;

namespace NPC_Generator
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

		internal static void BuildHumanNPC()
        {
            var temp = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject? tempplayer = null;
            foreach (var o in temp)
            {
                if (o.GetComponent<Player>() != null)
                { 
	                NetworkedNPC = Object.Instantiate(o, RootGOHolder?.transform!);
                    tempplayer = o;
                    break;
                }
            }
            Object.DestroyImmediate(NetworkedNPC?.GetComponent<PlayerController>());
            Object.DestroyImmediate(NetworkedNPC?.GetComponent<Player>());
            Object.DestroyImmediate(NetworkedNPC?.GetComponent<Talker>());
            Object.DestroyImmediate(NetworkedNPC?.GetComponent<Skills>());
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
            NetworkedNPC.AddComponent<TameHelper>();
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

		#endregion
    }
}