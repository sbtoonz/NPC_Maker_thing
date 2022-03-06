using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NPC_Generator
{
    public class YMLParser
    {
        public static string Serializers(Dictionary<string, NPCYamlConfig> data)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var yml = serializer.Serialize(data);
            return yml;
        }

        public static Dictionary<string, NPCYamlConfig> ReadSerializedData(string s)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var tmp = deserializer.Deserialize<Dictionary<string, NPCYamlConfig>>(s);
            return tmp;
        }
    }

    public struct NPCYamlConfig
    {
        [YamlMember(Alias = "Sex", ApplyNamingConventions = false)]
        public string npcSex { get; set; }
        
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
        
        [YamlMember(Alias = "HuntPlayer", ApplyNamingConventions = false)]
        public bool npcHuntPlayer { get; set; }

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
    }


}