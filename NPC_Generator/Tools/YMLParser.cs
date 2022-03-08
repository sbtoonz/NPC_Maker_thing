using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NPC_Generator.Tools
{
    public class YMLParser
    {
        /// <summary>
        /// Serializer for YML data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Serializers(Dictionary<string, NPCYamlConfig> data)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var yml = serializer.Serialize(data);
            return yml;
        }

        /// <summary>
        /// Deserializer for YML data
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Dictionary<string, NPCYamlConfig> ReadSerializedData(string s)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var tmp = deserializer.Deserialize<Dictionary<string, NPCYamlConfig>>(s);
            return tmp;
        }
    }

    /// <summary>
    /// This is the Struct template used to base all NPC configuration through
    /// </summary>
    public struct NPCYamlConfig
    {
        [YamlMember(Alias = "Sex", ApplyNamingConventions = false)]
        public string npcSex { get; set; }
        
        [YamlMember(Alias = "Health", ApplyNamingConventions = false)]
        public int mHealth { get; set; }
        
        [YamlMember(Alias = "Helmet Prefab Name", ApplyNamingConventions = false)]
        public string npcHelmetString { get; set; }
        
        [YamlMember(Alias = "Chest Armor Prefab Name", ApplyNamingConventions = false)]
        public string npcChestString { get; set; }
        
        [YamlMember(Alias = "Legs Armor Prefab Name", ApplyNamingConventions = false)]
        public string npcLegString { get; set; }
         
        [YamlMember(Alias = "Shoulder Armor Prefab Name", ApplyNamingConventions = false)]
        public string npcShoulder { get; set; }
        
        [YamlMember(Alias = "Weapon Prefab Name", ApplyNamingConventions = false)]
        public string npcWeapon { get; set; }
        
        [YamlMember(Alias = "Shield Prefab Name", ApplyNamingConventions = false)]
        public string npcShield { get; set; }
        
        [YamlMember(Alias = "Faction", ApplyNamingConventions = false)]
        public string npcFaction { get; set; }

        [YamlMember(Alias = "Scale", ApplyNamingConventions = false)]
        public float npcScale { get; set; }
        
        [YamlMember(Alias = "Tamable", ApplyNamingConventions = false)]
        public bool npcTameable { get; set; }
        
        [YamlMember(Alias = "Hair Style", ApplyNamingConventions = false)]
        public string npcHairStyle { get; set; }
        
        [YamlMember(Alias = "Skin Color R", ApplyNamingConventions = false)]
        public float npcSkinColorR { get; set; }
        
        [YamlMember(Alias = "Skin Color G", ApplyNamingConventions = false)]
        public float npcSkinColorG { get; set; }
        
        [YamlMember(Alias = "Skin Color B", ApplyNamingConventions = false)]
        public float npcSkinColorB { get; set; }
        
        [YamlMember(Alias = "Hair Color R", ApplyNamingConventions = false)]
        public float npcHairColorR { get; set; }
        
        [YamlMember(Alias = "Hair Color G", ApplyNamingConventions = false)]
        public float npcHairColorG { get; set; }
        
        [YamlMember(Alias = "Hair Color B", ApplyNamingConventions = false)]
        public float npcHairColorB { get; set; }
        
         [YamlMember(Alias = "Tolerate Water", ApplyNamingConventions = false)]
         public bool mTolerateWater { get; set; }
         
         [YamlMember(Alias = "Tolerate Fire", ApplyNamingConventions = false)]
         public bool mTolerateFire { get; set; }
         
         [YamlMember(Alias = "Tolerate Smoke", ApplyNamingConventions = false)]
         public bool mTolerateSmoke { get; set; }
         
         [YamlMember(Alias = "Tolerate Tar", ApplyNamingConventions = false)]
         public bool mTolerateTar { get; set; }
         
         [YamlMember(Alias = "NPC In Game Name", ApplyNamingConventions = false)]
         public string mNPCInGameName { get; set; }
        

        [YamlMember(Alias = "Damage Resists", ApplyNamingConventions = false)]
        public DamageResists DamageResists { get; set; }
        
        [YamlMember(Alias = "Monster AI Config", ApplyNamingConventions = false)]
        public MonsterAIConfig monsterAiConfig { get; set; }
        
        [YamlMember(Alias = "Drop Config", ApplyNamingConventions = false)]
        public DropConfig DropConfig { get; set; }
        
        [YamlMember(Alias = "Villager Config", ApplyNamingConventions = false)]
        public VillagerConfig villagerConfig { get; set; }

    }

    public struct DamageResists
    {
        [YamlMember(Alias = "Blunt", ApplyNamingConventions = false)]
        public string mBlunt { get; set; }

        [YamlMember(Alias = "Slash", ApplyNamingConventions = false)]
        public string mSlash { get; set;}
        
        [YamlMember(Alias = "Pierce", ApplyNamingConventions = false)]
        public string mPierce { get; set; }
        
        [YamlMember(Alias = "Chop", ApplyNamingConventions = false)]
        public string mChop { get; set; }
        
        [YamlMember(Alias = "Pickaxe", ApplyNamingConventions = false)]
        public string mPickaxe { get; set; }
        
        [YamlMember(Alias = "Fire", ApplyNamingConventions = false)]
        public string mFire { get; set; }
        
        [YamlMember(Alias = "Frost", ApplyNamingConventions = false)]
        public string mFrost { get; set; }
        
        [YamlMember(Alias = "Lightning", ApplyNamingConventions = false)]
        public string mLightning { get; set; }
        
        [YamlMember(Alias = "Poison", ApplyNamingConventions = false)]
        public string mPoison { get; set; }
        
        [YamlMember(Alias = "Spirit", ApplyNamingConventions = false)]
        public string mSpirit { get; set; }
    }

    public struct MonsterAIConfig
    {
        [YamlMember(Alias = "View Range",ApplyNamingConventions = false)]
        public int mViewRange { get; set; }
        
        [YamlMember(Alias = "View Angle",ApplyNamingConventions = false)]
        public int mViewAngle { get; set; }
        
        [YamlMember(Alias = "Hear Range",ApplyNamingConventions = false)]
        public int mHearRange { get; set; }
        
        [YamlMember(Alias = "Alert Range",ApplyNamingConventions = false)]
        public int mAlertRange { get; set; }
        
        [YamlMember(Alias = "Flee if hurt cant be reached", ApplyNamingConventions = false)]
        public bool mFleeifHurt { get; set; }
        
        [YamlMember(Alias = "Flee if not alerted", ApplyNamingConventions = false)]
        public bool mFleeNotAlert { get; set; }
        
        [YamlMember(Alias = "Flee low health", ApplyNamingConventions = false)]
        public int mFleeLowHealth { get; set; }
        
        [YamlMember(Alias = "Circulate while Charging", ApplyNamingConventions = false)]
        public bool mCirculateCharge { get; set; }

        [YamlMember(Alias = "Attack player objects", ApplyNamingConventions = false)]
        public bool mAttackPlayerObj { get; set; }
        
        [YamlMember(Alias = "Intercept Time Max", ApplyNamingConventions = false)]
        public int mInterceptMax { get; set; }
        
        [YamlMember(Alias = "Intercept Time Min", ApplyNamingConventions = false)]
        public int mInterceptMin { get; set; }
        
        [YamlMember(Alias = "Chase Distance", ApplyNamingConventions = false)]
        public int mChaseDistance { get; set; }
        
        [YamlMember(Alias = "Attack Interval", ApplyNamingConventions = false)]
        public int mAttackInterval { get; set; }
        
        [YamlMember(Alias = "Circle Target Interval", ApplyNamingConventions = false)]
        public int mCircleInteverval { get; set; }
        
        [YamlMember(Alias = "Circulate Target Duration", ApplyNamingConventions = false)]
        public int mCircleDuration { get; set; }
        
        [YamlMember(Alias = "Circle Target Distance", ApplyNamingConventions = false)]
        public int mCircleDistance { get; set; }
        
        [YamlMember(Alias = "Hunt Player", ApplyNamingConventions = false)]
        public bool npcHuntPlayer { get; set; }
        
        [YamlMember(Alias = "Avoid Water", ApplyNamingConventions = false)]
        public bool npcAvoidWater { get; set; }
        
        [YamlMember(Alias = "Avoid Land", ApplyNamingConventions = false)]
        public bool npcAvoidLand { get; set; }
        
        [YamlMember(Alias = "Avoid Fire", ApplyNamingConventions = false)]
        public bool npcAvoidFire { get; set; }

        [YamlMember(Alias = "Random Move Interval", ApplyNamingConventions = false)]
        public float npcRandomMoveInterval{ get; set; }

        [YamlMember(Alias = "Random Move Range", ApplyNamingConventions = false)]
        public float npcRandomMoveRange { get; set; }


    }

    public struct DropConfig
    {
        [YamlMember(Alias = "DropTable", ApplyNamingConventions = false)]
        public Dictionary<string, DropItem> DropItems { get; set; }
    }

    public struct DropItem
    {
        [YamlMember(Alias = "Drop Chance", ApplyNamingConventions = false)]
        public float m_chance { get; set; }
        
        [YamlMember(Alias = "One Per Player", ApplyNamingConventions = false)]
        public bool m_onePer { get; set; }
        
        [YamlMember(Alias = "Amount Max Chance", ApplyNamingConventions = false)]
        public int m_ammountMax { get; set; }
        
        [YamlMember(Alias = "Amount Min Chance", ApplyNamingConventions = false)]
        public int m_ammountMin { get; set; }
        
        [YamlMember(Alias = "Multiply for multiple Players", ApplyNamingConventions = false)]
        public bool m_Multply { get; set; }
    }

    public struct VillagerConfig
    {
        [YamlMember(Alias = "Is Basic Villager",ApplyNamingConventions = false)]
        public bool mIsVillager { get; set; }
        
        [YamlMember(Alias = "Villager Builder", ApplyNamingConventions = false)]
        public bool mVillagerBuilder { get; set; }
        
        [YamlMember(Alias = "Villager Farmer", ApplyNamingConventions = false)]
        public bool mVillagerFarmer { get; set; }
        
    }
    

}