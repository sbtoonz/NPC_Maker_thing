using NPC_Generator.RPC;
using NPC_Generator.NPC_Utilities;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace NPC_Generator.MonoScripts.Villagers;

public class VillagerRandEvent : VillagerBase
{
    public override bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (alt)
        {
            return false;
        }
        if (hold)
        {
            return false;
        }
        if(m_nview.GetZDO().GetBool("RaidStarted") == false)
        {
            Say("The neighboring village is attacking!");
            ZRoutedRpc.instance.InvokeRoutedRPC("RPC_Villager_Raid", transform.position);
            m_nview.GetZDO().Set("RaidStarted", true);
            return true;
        }
        else
        {
            if (RPCs.randomEvent == null)
            {
                Say("Many thanks for your assistance");
                return false;
            }
            
            if (RPCs.randomEvent.m_active)
            {
                Say("Help fend off the raiders");
                return true;
            }
            else
            {
                Say("Thank you for the help!");
                if (m_nview.GetZDO().GetBool("GivenReward") == false)
                {
                    Vector3 vector = Random.insideUnitSphere * 0.5f;
                    var coin = Instantiate(ZNetScene.instance.GetPrefab("Coins"), transform.position + transform.forward * 2f + Vector3.up + vector, Quaternion.identity);
                    var id = coin.GetComponent<ItemDrop>();
                    id.SetStack(id.m_itemData.m_shared.m_maxStackSize.RollDice());
                    m_nview.GetZDO().Set("GivenReward", true);
                }
                return true;
            }
        }
        
    }
}