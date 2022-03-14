using System;
using System.Collections.Generic;
using UnityEngine;
namespace NPC_Generator.MobAILib;
using RagnarsRokare.MobAI;
public class NPCMobAI : MonoBehaviour
{
    private static MobAIBaseConfig testconfig = new SorterAIConfig();
    public Character m_character;
    
    private void Awake()
    {
        m_character = GetComponent<Character>();
        testconfig.AIStateCustomStrings = AIStateDictionary;
        testconfig.Agressiveness = 3;
        testconfig.Mobility = 10;
        testconfig.Awareness = 20;
        testconfig.Intelligence = 100;
        MobManager.RegisterMob(m_character, Guid.NewGuid().ToString(), "Worker Human", testconfig);
    }
    
    public static Dictionary<string, string> AIStateDictionary { get; } = new Dictionary<string, string>()
    {
        {"RR_FIGHTMain","HUH?"},
        {"RR_FIGHTIdentifyEnemy", "ME BASH DIS {0}"},
        {"RR_FIGHTDoneFighting","GUD BASH!"},
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
        {"Flee", "Takin a short breather"},
        {"Follow", "Follow punyboss"},
        {"Assigned", "uuhhhmm..  checkin' dis over 'ere"},
        {"MoveToAssignment", "Moving to assignment {0}"},
        {"CheckRepairState","Naah dis {0} goood"},
        {"RepairAssignment","Fixin Dis {0}"}
    };
}