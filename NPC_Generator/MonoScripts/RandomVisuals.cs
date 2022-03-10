using NPC_Generator.NPC_Utilities;
using UnityEngine;

namespace NPC_Generator.MonoScripts
{
    public class RandomVisuals : MonoBehaviour
    {
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
			m_vis.CleanupInstance(gameObject);
			m_vis.SetHairItem("");
			m_vis.SetHairItem(m_hairItem.GetRandomElement());
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