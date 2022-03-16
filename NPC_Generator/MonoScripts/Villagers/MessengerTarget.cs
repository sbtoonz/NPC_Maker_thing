using NPC_Generator.NPC_Utilities;
using UnityEngine;

namespace NPC_Generator.MonoScripts.Villagers
{
    public class MessengerTarget : VillagerBase
    {
        public ZNetView? m_sendernview;
        public static string[] NPCnames = { "$op_npc_name1", "$op_npc_name2", "$op_npc_name3", "$op_npc_name4", "$op_npc_name5", "$op_npc_name6", "$op_npc_name7", "$op_npc_name8", "$op_npc_name9", "$op_npc_name10", "$op_npc_name11", "$op_npc_name12", "$op_npc_name13", "$op_npc_name14", "$op_npc_name15", "$op_npc_name16", "$op_npc_name17", "$op_npc_name18", "$op_npc_name19", "$op_npc_name20", "$op_npc_name21", "$op_npc_name22", "$op_npc_name23", "$op_npc_name24", "$op_npc_name25", "$op_npc_name26", "$op_npc_name27", "$op_npc_name28", "$op_npc_name29", "$op_npc_name30", "$op_npc_name31", "$op_npc_name32", "$op_npc_name33", "$op_npc_name34", "$op_npc_name35", "$op_npc_name36", "$op_npc_name37", "$op_npc_name38", "$op_npc_name39", "$op_npc_name40", "$op_npc_name41", "$op_npc_name42", "$op_npc_name43", "$op_npc_name44", "$op_npc_name45", "$op_npc_name46", "$op_npc_name47", "$op_npc_name48", "$op_npc_name49", "$op_npc_name50" };
        public TimedDestruction? timedest { get; set; }
        internal CapsuleCollider? playerCollider;
        public override void Awake()
        {
            base.Awake();
            m_name = NPCnames.GetRandomElement();
            playerCollider = GetComponent<CapsuleCollider>();
            playerCollider.enabled = true;
        }

        public override bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (m_sendernview!.GetZDO().GetBool("QuestActive"))
            {
                hum.m_group = Player.m_localPlayer.m_group;
                string n = string.Format("Thanks for delivering the message");
                Say(n, "emote_thumbsup");
                m_sendernview.GetZDO().Set("QuestActive", false);
                Vector3 vector = Random.insideUnitSphere * 0.5f;
                var coin = Instantiate(ZNetScene.instance.GetPrefab("Coins"), transform.position + transform.forward * 2f + Vector3.up + vector, Quaternion.identity);
                var id = coin.GetComponent<ItemDrop>();
                id.SetStack(id.m_itemData.m_shared.m_maxStackSize.RollDice());
                timedest = gameObject.AddComponent<TimedDestruction>();
                timedest.m_nview = gameObject.GetComponent<ZNetView>();
                timedest.m_triggerOnAwake = true;
                timedest.Awake();
                return true;
            }
            else
            {
                return false;
            }
            
        }
        
        public override bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return false;
        }
        
        public override void Say(string text, string emote)
        {
            if (hum.m_faction != Character.Faction.Players)
            {
                return;
            }
            var tname = Localization.instance.Localize(m_name);
            Chat.instance.SetNpcText(m_talker, Vector3.up * 1.5f, 60f, 5, tname, text, false);
            m_ani.SetTrigger(emote);
        }
    }
}