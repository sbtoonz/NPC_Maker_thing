using System.Collections.Generic;
using RagnarsRokare.MobAI;
using UnityEngine;

namespace NPC_Friendlies.MobAIBehaviors;
public class MobConfig
{
    public IEnumerable<ItemDrop> PreTameConsumables { get; set; }
    public IEnumerable<ItemDrop> PostTameConsumables { get; set; }
    public float PreTameFeedDuration { get; set; }
    public float PostTameFeedDuration { get; set; }
    public float TamingTime { get; set; }
    public string AIType { get; set; }
    public MobAIBaseConfig AIConfig { get; set; }
}

public class MobAIBehaviorGather
{
    private static MobAIBaseConfig sorterConfig = new SorterAIConfig();
    private static MobAIBase sorterBase = new SorterAI();
    private static WorkerAIConfig gatherConfig = new WorkerAIConfig();
    private static WorkerAI _workerAI = new WorkerAI();
    
    public static Dictionary<string, string> AIStateDictionary { get; } = new Dictionary<string, string>()
    {
        {"RR_FIGHTMain","hmm?"},
        {"RR_FIGHTIdentifyEnemy", "EIIII See {0}!"},
        {"RR_FIGHTDoneFighting","*looks relieved*"},
        {"RR_EATHungry", "Is hungry, no work a do"},
        {"RR_EATHaveFoodItem","*burps*"},
        {"RR_ISBMoveToContainer", "Heading to that a bin"},
        {"RR_ISBMoveToStorageContainer", "Heading to that a bin"},
        {"RR_ISBMoveToGroundItem", "Heading to {0}"},
        {"RR_ISBPickUpItemFromGround", "Got a {0} from the ground"},
        {"RR_ISBSearchItemsOnGround", "Look, there is a {0} on da grund"},
        {"RR_SFISearchItemsOnGround","Look, there is a {0} on da grund"},
        {"RR_SFISearchForRandomContainer","Look a bin!"},
        {"RR_SFIMoveToGroundItem","Heading to {0}"},
        {"RR_SFIMoveToPickable","Heading to {0}"},
        {"RR_SFIPickUpItemFromGround","Got a {0} from the ground"},
        {"RR_SFIMoveToContainer","Heading to that a bin"},
        {"RR_SFISearchForItem","Found {0} in this a bin!"},
        {"Idle", "Nothing to do, bored"},
        {"Flee", "AOWEEE!"},
        {"Follow", "Follow bigboss"},
        {"MoveAwayFrom", "Ahhh Scary!"},
        {"Assigned", "I'm on it Boss" },
        {"HaveAssignment", "Trying to Pickup {0}"},
        {"MoveToAssignment", "Moving to assignment {0}"},
        {"CheckingAssignment","Chekkin dis {0}"},
        {"UnloadToAssignment","Stuffin dis {0} full"},
        {"DoneWithAssignment", "Done doin worksignment!"}
    };

    public static HashSet<string> WorkableAssignments { get; set; }

    public static MobConfig MobConfig()
    {
        return new MobConfig
        {
            PostTameConsumables = CreateDropItemList(ModCore.PostTameConsumables),
            PostTameFeedDuration = ModCore.FeedDuration.Value,
            PreTameConsumables = CreateDropItemList(ModCore.PreTameConsumables),
            PreTameFeedDuration = ModCore.FeedDuration.Value,
            TamingTime = ModCore.TamingTime.Value,
            AIType = "Worker",
            AIConfig = new WorkerAIConfig
            {
                FeedDuration = ModCore.FeedDuration.Value,
                IncludedContainers = ModCore.IncludedContainersList.Value.Replace(" ", "").Split(',', ';'),
                TimeLimitOnAssignment = ModCore.TimeLimitOnAssignment.Value,
                Agressiveness = ModCore.Agressiveness.Value,
                Awareness = ModCore.Awareness.Value,
                Mobility = ModCore.Mobility.Value,
                Intelligence = ModCore.Intelligence.Value,
                AIStateCustomStrings = AIStateDictionary,
                WorkableAssignments = WorkableAssignments
            }
        };
    }
    
    public static IEnumerable<ItemDrop> CreateDropItemList(IEnumerable<string> itemNames)
    {
        foreach (var itemName in itemNames)
        {
            var item = ObjectDB.instance.GetItemByName(itemName);
            if (null == item)
            {
                Debug.LogWarning($"Cannot find item {itemName} in objectDB");
                continue;
            }
            yield return item;
        }
    }
}