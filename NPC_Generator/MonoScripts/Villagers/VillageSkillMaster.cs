using System;
using System.Collections.Generic;
using NPC_Generator.NPC_Utilities;
using NPC_Generator.Tools;
using UnityEngine;

namespace NPC_Generator.MonoScripts.Villagers;

public class VillageSkillMaster : VillagerBase
{
#pragma warning disable CS8618
    [SerializeField] internal string itemName = "";
    [SerializeField] internal string skillName = "";
    [SerializeField] internal int reqstack = 1;
    [SerializeField] internal int minSkillGain = 1;
    [SerializeField] internal int maxSkillGain = 2;
#pragma warning restore CS8618
    
    internal ItemDrop.ItemData? _itemData;
    internal Skills.SkillType _skillType;
    private bool levelGiven = false;
    
    public override void Awake()
    {
        base.Awake();
        _itemData = ZNetScene.instance.GetPrefab(itemName).GetComponent<ItemDrop>().m_itemData;
        _skillType = skillName.ToLower() switch
        {
            "swords" => Skills.SkillType.Swords,
            "knives" => Skills.SkillType.Knives,
            "clubs" => Skills.SkillType.Clubs,
            "polearms" => Skills.SkillType.Polearms,
            "spears" => Skills.SkillType.Spears,
            "blocking" => Skills.SkillType.Blocking,
            "axes" => Skills.SkillType.Axes,
            "bows" => Skills.SkillType.Bows,
            "unarmed" => Skills.SkillType.Unarmed,
            "pickaxes" => Skills.SkillType.Pickaxes,
            "woodcutting" => Skills.SkillType.WoodCutting,
            "jump" => Skills.SkillType.Jump,
            "sneak" => Skills.SkillType.Sneak,
            "run" => Skills.SkillType.Run,
            "swim" => Skills.SkillType.Swim,
            "ride" => Skills.SkillType.Ride,
            "all" => Skills.SkillType.All,
            _ => _skillType
        };
    }

    public override bool UseItem(Humanoid user, ItemDrop.ItemData item)
    {
        if(m_nview.GetZDO().GetBool("levelQuest"))
        {
            if (!user.m_inventory.HaveItem(item.m_shared.m_name) || user.m_inventory.GetItem(item.m_shared.m_name).m_stack <= reqstack) return false;
            user.m_inventory.RemoveItem(item, reqstack);
            user.RaiseSkill(_skillType, maxSkillGain.RollDice());
            Say("Thank you for the item, enjoy your blessing kind traveler", "emote_thumbsup");
            levelGiven = true;
            m_nview.GetZDO().Set("levelQuest", false);
            return true;
        }
        return false;

    }

    public override bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (!levelGiven)
        {
            Say("I Require" + reqstack + Localization.instance.Localize(_itemData!.m_shared.m_name));
            m_nview.GetZDO().Set("levelQuest", true);
            return true;
        }
        if (m_nview.GetZDO().GetBool("levelQuest"))
        {
            Say("Please locate me" + reqstack + Localization.instance.Localize(_itemData!.m_shared.m_name) + " For a blessing");
            return true;
        }
        if (m_nview.GetZDO().GetBool("levelQuest") == false)
        {
            return false;
        }
        
        return false;
    }
}