using System.Numerics;
using Vector3 = UnityEngine.Vector3;

namespace NPC_Generator.MonoScripts.Villagers
{
    public class Villager_Messenger : VillagerBase
    {
        private void PlaceQuestHuman(string key, Vector3 pos)
        {
            var pgo = ZNetScene.instance.GetPrefab("WorkerNPCHuman");
            var go  = Instantiate(pgo,NPC_Generator.RootGOHolder.transform);
            float y;
            ZoneSystem.instance.FindFloor(pos,out y);
            pos = new Vector3(pos.x,y+2,pos.z);
            go.transform.localPosition = pos;
            go.transform.SetParent(transform.parent.parent);
            NPC_Generator.DebugLog(NPC_Generator.DebugLevel.All,"Place Quest Worker at " + pos);
        }
        private bool PlaceRandom(string key)
        {
            foreach (var item in ZoneSystem.instance.m_locations)
            {
                var dis = Utils.DistanceXZ(item.m_location.transform.position, transform.position);
                if (dis>100)
                {
                    PlaceQuestHuman(key,item.m_location.transform.position);
                    return true;
                }
            }
            return false;
        }

        public override bool Interact(Humanoid user, bool hold, bool alt)
        {
            return false;
        }
        
        public override bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return false;
        }
        
    }
}