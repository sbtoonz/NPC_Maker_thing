using System.Collections.Generic;

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
        ZRoutedRpc.instance.InvokeRoutedRPC("RPC_Villager_Raid", transform.position);
        return true;
    }

    public override bool UseItem(Humanoid user, ItemDrop.ItemData item)
    {
        return false;
    }

}