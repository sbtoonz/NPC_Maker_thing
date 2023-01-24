using NPC_Generator.NPC_Utilities;
using NPC_Generator.Tools;
using UnityEngine;

namespace NPC_Generator.MonoScripts.Villagers;

public class VillageSkillMaster : VillagerBase
{
#pragma warning disable CS8618
    [SerializeField] internal string[] itemNames;
    [SerializeField] public string m_item = "";
    [SerializeField] internal string[] skillNames;
    [SerializeField] internal string m_skill = "";
    [SerializeField] internal int reqstack = 1;
    [SerializeField] internal int minSkillGain = 1;
    [SerializeField] internal int maxSkillGain = 2;
#pragma warning restore CS8618
    
    internal ItemDrop.ItemData? _itemData;
    internal Skills.SkillType _skillType;
    internal bool QuestReady => IsQuestReady();
    
    public override void Awake()
    {
        base.Awake();
        var zdo = m_nview.GetZDO();
        m_item = zdo.GetString("Qmat","");
        if (m_item=="")
        {
            m_item=itemNames.GetRandomElement();
            zdo.Set("Qmat",m_item);
        }
        m_skill = zdo.GetString("QSkill", "");
        if (m_skill == "")
        {
            m_skill = skillNames.GetRandomElement().ToLower();
            zdo.Set("QSkill",m_skill);
        }
        _itemData = ZNetScene.instance.GetPrefab(m_item).GetComponent<ItemDrop>().m_itemData;
        _skillType = m_skill switch
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
        if (!QuestReady)
        {
            return false;
        }
        if(!IsQuestDone())
        {
            if (!user.m_inventory.HaveItem(item.m_shared.m_name) || user.m_inventory.GetItem(item.m_shared.m_name).m_stack <= reqstack) return false;
            user.m_inventory.RemoveItem(item, reqstack);
            Player player = user.gameObject.GetComponent<Player>();
            player.m_skills.CheatRaiseSkill(_skillType.ToString(), maxSkillGain.RollDice());
            Say("Thank you for the item, enjoy your blessing kind traveler", "emote_thumbsup");
            m_nview.GetZDO().Set("levelQuest", false);
            ResetQuestCD();
            return true;
        }
        return false;

    }

    public override bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (!IsQuestDone())
        {
            Say("Please locate me " + "<color=yellow>"+reqstack+"</color>" + " " + Localization.instance.Localize(_itemData!.m_shared.m_name) + " For a blessing");
            return true;
        }
        else
        {
            return false;
        }
    }
}