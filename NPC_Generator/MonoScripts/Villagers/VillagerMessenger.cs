using System.Numerics;
using NPC_Generator.RPC;
using Vector3 = UnityEngine.Vector3;

namespace NPC_Generator.MonoScripts.Villagers
{
    public class VillagerMessenger : VillagerBase
    {
        private void PlaceQuestHuman(Vector3 pos)
        {
            var pgo = gameObject;
            var go  = Instantiate(pgo, NPC_Generator.RootGOHolder!.transform);
            DestroyImmediate(go.GetComponent<HairSetter>());
            DestroyImmediate(go.GetComponent<SkinColorHelper>());
            DestroyImmediate(go.GetComponent<FemaleAssigner>());
            DestroyImmediate(go.GetComponent<VillagerMessenger>());
            go.AddComponent<RandomVisuals>();
            var target =go.AddComponent<MessengerTarget>();
            target.m_sendernview = m_nview;
            float y;
            ZoneSystem.instance.FindFloor(pos,out y);
            pos = new Vector3(pos.x,y+2,pos.z);
            go.transform.localPosition = pos;
            go.transform.SetParent(Game.instance.transform.parent);
            NPC_Generator.DebugLog(NPC_Generator.DebugLevel.All,"Place Quest Worker at " + pos);
            m_nview.GetZDO().Set("QuestActive", true);
        }

        public override bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (hold)
            {
                return false;
                
            }
            if (alt)
            {
                if (!m_nview.GetZDO().GetBool("QuestActive"))
                {
                    ZRoutedRpc.instance.InvokeRoutedRPC("RPC_Find_Location", Player.m_localPlayer.transform.position);
                    Invoke(nameof(slowInvoke), 1f);
                }
                else
                {
                    return false;
                }
                
            }
            if (!m_nview.GetZDO().GetBool("QuestActive"))
            {
                Say("Can you take a message to a nearby village for me?", "emote_wave");
                return true;
            }
            else
            {
                return false; 
            }

        }
        private void slowInvoke()
        {
           PlaceQuestHuman(RPCs.position); 
        }
        public override bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return false;
        }
        
        public override string GetHoverText()
        {
            if (!m_nview.GetZDO().GetBool("QuestActive"))
            {
                string s = Localization.instance.Localize("[$button_lshift]");
                s += " and <color=yellow>[E]</color> to accept mission";
                return s;
            }
            else
            {
                return m_name;
            }
        }
    }
}