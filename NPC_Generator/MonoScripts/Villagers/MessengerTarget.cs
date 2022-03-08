using UnityEngine;

namespace NPC_Generator.MonoScripts.Villagers
{
    public class MessengerTarget : VillagerBase
    {
        public ZNetView m_sendernview;

        public override bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (m_sendernview.GetZDO().GetBool("QuestActive"))
            {
                Say("Thank you for the message", "emote_thumbsup");
                m_sendernview.GetZDO().Set("QuestActive", false);
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
    }
}