using NPC_Generator.NPC_Utilities;
using UnityEngine;
using static NPC_Generator.NPC_Utilities.NpcUtilities;

namespace NPC_Generator.MonoScripts.Villagers
{
    public class Villager_Builder : VillagerBase
    {
        public string m_item = "";
        public readonly string[] m_materials = new string[] { "Wood", "Stone", "FineWood", "RoundLog" };
        internal bool QuestReady => IsQuestReady();
        public override void Awake()
        {
            base.Awake();
            var zdo = m_nview.GetZDO();
            m_item = zdo.GetString("Qmat","");
            if (m_item=="")
            {
                m_item=m_materials.GetRandomElement();
                zdo.Set("Qmat",m_item);
            }
            if (QuestReady)
            {
                
            }
            gameObject.GetComponent<MonsterAI>().RandomMovement(1, transform.position);
        }

        public override bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (hold)
            {
                return false;
            }
            if (!IsQuestDone())
            {
                int count = GetItemData(m_item).m_shared.m_maxStackSize;
                string n = string.Format("I could use <color=red><b>{1}</b></color> <color=yellow><b>{0}</b></color> to build our home",m_item, count);
                Say(n, "emote_wave");
                return true;  
            }
            else
            {
                return false;
            }
            
        }
        
        public override bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            if (!QuestReady)
            {
                return false;
            }
            if(!IsQuestDone())
            {
                var inv = Player.m_localPlayer.GetInventory();
                string iname = GetItemData(m_item).m_shared.m_name;
                int count = GetItemData(m_item).m_shared.m_maxStackSize;
                if (inv.CountItems(iname) >= count)
                {
                    inv.RemoveItem(iname, count);
                    Say("Thanks!", "emote_point");
                    Vector3 vector = Random.insideUnitSphere * 0.5f;
                    var coin = Instantiate(ZNetScene.instance.GetPrefab("Coins"), transform.position + transform.forward * 2f + Vector3.up + vector, Quaternion.identity);
                    var id = coin.GetComponent<ItemDrop>();
                    id.SetStack(id.m_itemData.m_shared.m_maxStackSize.RollDice());
                    ResetQuestCD();
                    return true;
                }
                else
                {
                    Say("I could use a bit more", "emote_nonono");
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}