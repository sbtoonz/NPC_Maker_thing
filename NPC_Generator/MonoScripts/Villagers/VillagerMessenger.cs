using System.Numerics;
using Vector3 = UnityEngine.Vector3;

namespace NPC_Generator.MonoScripts.Villagers
{
    public class VillagerMessenger : VillagerBase
    {
        private void PlaceQuestHuman(Vector3 pos)
        {
            var pgo = gameObject;
            var go  = Instantiate(pgo, NPC_Generator.RootGOHolder!.transform);
            var hair = go.GetComponent<HairSetter>();
            Destroy(hair);
            var skin = go.GetComponent<SkinColorHelper>();
            Destroy(skin);
            var femassign = GetComponent<FemaleAssigner>();
            Destroy(femassign);
            float y;
            var messeneger = go.GetComponent<VillagerMessenger>();
            Destroy(messeneger);
            go.AddComponent<RandomVisuals>();
            var target =go.AddComponent<MessengerTarget>();
            target.m_sendernview = m_nview;
            ZoneSystem.instance.FindFloor(pos,out y);
            pos = new Vector3(pos.x,y+2,pos.z);
            go.transform.localPosition = pos;
            go.transform.SetParent(Game.instance.transform.parent);
            NPC_Generator.DebugLog(NPC_Generator.DebugLevel.All,"Place Quest Worker at " + pos);
            m_nview.GetZDO().Set("QuestActive", true);
        }

        private bool RandomPlacer()
        {
            var locations = ZoneSystem.instance.GetLocationList();

            foreach(var loc in locations)
            {

                float dist = Vector3.Distance(loc.m_position, Player.m_localPlayer.transform.position);
                if(dist < 100)
                {
                    if(loc.m_location.m_prefabName.Contains("House"))
                    {
                        PlaceQuestHuman(loc.m_position);
                        return true;
                    }
                }
            };
            Say("I can't find my friend right now... please check back with me later");
            return false;
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
                    RandomPlacer();
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