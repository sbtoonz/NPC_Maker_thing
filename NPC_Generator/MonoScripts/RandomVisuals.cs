﻿using NPC_Generator.NPC_Utilities;
using UnityEngine;

namespace NPC_Generator.MonoScripts
{
    public class RandomVisuals : MonoBehaviour
    {
        //public static string[] NPCnames = { "$op_npc_name1", "$op_npc_name2", "$op_npc_name3", "$op_npc_name4", "$op_npc_name5", "$op_npc_name6", "$op_npc_name7", "$op_npc_name8", "$op_npc_name9", "$op_npc_name10", "$op_npc_name11", "$op_npc_name12", "$op_npc_name13", "$op_npc_name14", "$op_npc_name15", "$op_npc_name16", "$op_npc_name17", "$op_npc_name18", "$op_npc_name19", "$op_npc_name20", "$op_npc_name21", "$op_npc_name22", "$op_npc_name23", "$op_npc_name24", "$op_npc_name25", "$op_npc_name26", "$op_npc_name27", "$op_npc_name28", "$op_npc_name29", "$op_npc_name30", "$op_npc_name31", "$op_npc_name32", "$op_npc_name33", "$op_npc_name34", "$op_npc_name35", "$op_npc_name36", "$op_npc_name37", "$op_npc_name38", "$op_npc_name39", "$op_npc_name40", "$op_npc_name41", "$op_npc_name42", "$op_npc_name43", "$op_npc_name44", "$op_npc_name45", "$op_npc_name46", "$op_npc_name47", "$op_npc_name48", "$op_npc_name49", "$op_npc_name50" };
		public string[] m_beardItem = { "Beard2", "Beard3", "Beard4", "Beard5", "Beard6", "Beard7", "Beard8", "Beard9", "Beard10" };
		public string[] m_hairItem = { "Hair1", "Hair2", "Hair3", "Hair4", "Hair5", "Hair6", "Hair7", "Hair8", "Hair9", "Hair10" };
		protected ZNetView m_nview;
		protected VisEquipment m_vis;
		protected Animator m_ani;
		public string m_name="";
		private Humanoid _humanoid;
		private void Awake()
		{
			m_nview = GetComponent<ZNetView>();
			m_ani = GetComponentInChildren<Animator>();
			m_vis = GetComponent<VisEquipment>();
			_humanoid = GetComponent<Humanoid>();
			
			NpcUtilities.seed += (int)((gameObject.transform.position.x + gameObject.transform.position.y) * 1000);
			m_ani.SetBool("wakeup", false);
			SetupVisual();
		}
		protected virtual void SetupVisual()
		{
			int sex = 2.RollDice();
			if (sex == 0)
			{
				SetItem("BeardItem", m_beardItem);
			}
			SetItem("HairItem", m_hairItem);
			float skin = 0.5f + 0.8f.RollDice();
			Color hair = Color.HSVToRGB(0.13f + 0.03f.RollDice(), 1f.RollDice(), 1.3f.RollDice());
			m_vis.m_nview = m_nview;
			m_vis.SetModel(2.RollDice());
			m_vis.SetHairColor(new Vector3(hair.r, hair.g, hair.b));
			m_vis.SetSkinColor(new Vector3(skin, skin, skin));
			//m_vis.m_skinColor = new Vector3(1f.RollDices(), 1f.RollDices(), 1);
		}
		protected void SetItem(string slot, string[] items)
		{
			m_nview.GetZDO().Set(slot, items.GetRandomElement().GetStableHashCode());
		}
    }
}