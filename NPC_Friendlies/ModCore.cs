using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;

namespace NPC_Friendlies
{


    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("RagnarsRokare.MobAILib")]
    public class ModCore : BaseUnityPlugin
    {
        internal const string ModName = "NPC_Friendlies";
        internal const string ModVersion = "0.0.1";
        private const string ModGUID = "com.odinplus.NPC_Friendlies";
        private static Harmony harmony = null!;

        //
        internal static ConfigEntry<bool>? _serverConfigLocked;
        public static ConfigEntry<int> FeedDuration;
        public static ConfigEntry<string> BrutePrefabName;
        public static ConfigEntry<string> TamingItemList;
        public static ConfigEntry<string> HungryItemList;
        public static ConfigEntry<int> PreTameFeedDuration;
        public static ConfigEntry<int> PostTameFeedDuration;
        public static ConfigEntry<int> TamingTime;
        public static ConfigEntry<int> TimeLimitOnAssignment;
        public static ConfigEntry<string> IncludedContainersList;
        public static IEnumerable<string> PreTameConsumables;
        public static IEnumerable<string> PostTameConsumables;
        public static ConfigEntry<int> Awareness;
        public static ConfigEntry<int> Agressiveness;
        public static ConfigEntry<int> Mobility;
        public static ConfigEntry<int> Intelligence;
        private static ConfigSync configSync = new(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion};

        ConfigEntry<T> config<T>(string group, string configName, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = Config.Bind(group, configName, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        ConfigEntry<T> config<T>(string group, string configName, T value, string description, bool synchronizedSetting = true) => config(group, configName, value, new ConfigDescription(description), synchronizedSetting);

        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            harmony = new(ModGUID);
            harmony.PatchAll(assembly);
            SetupConfigs();
        }

        internal void SetupConfigs()
        {
            _serverConfigLocked = config("General", "Lock Configuration", false, "Lock Configuration");
            BrutePrefabName = config("General", "PrefabName", "Jim", "The prefab to use the repair ai with (repair structures)");
            TamingItemList = config("General", "TamingItemList", "Dandelion", "Comma separated list if items used to tame villager");
            HungryItemList = config("General", "PostTameConsumables", "QueensJam,Raspberry,Honey,Blueberries,Resin,Dandelion", "Comma separated list if items Brutes eat when hungry");
            FeedDuration = config("General", "Greyling_FeedDuration", 500, "Time before getting hungry after consuming one item");
            PreTameFeedDuration = config("General", "PreTameFeedDuration", 100, "Time before getting hungry after consuming one item during taming");
            PostTameFeedDuration = config("General", "PostTameFeedDuration", 1000, "Time before getting hungry after consuming one item when tame");
            TamingTime = config("General", "TamingTime", 1000, "Total time it takes to tame a Brute");
            TimeLimitOnAssignment = config("General", "TimeLimitOnAssignment", 30, "How long before moving on to next assignment");
            IncludedContainersList = config("General", "IncludedContainersList", "piece_chest_wood", "Comma separated list of container piece names to be searchable by Greylings");
            PreTameConsumables = TamingItemList.Value.Replace(" ", "").Split(',', ';');
            PostTameConsumables = HungryItemList.Value.Replace(" ", "").Split(',', ';');
            Awareness = config("General", "Awareness", 6, "General awareness, used to calculate search ranges and ability to detect enemies");
            Agressiveness = config("General", "Agressiveness", 8, "Agressivness determines how to behave when fighting and when to give up and flee");
            Mobility = config("General", "Mobility", 10, "Mobility is used to determine how often and how far the mob moves");
            Intelligence = config("General", "Intelligence", 5, "General intelligence, how much the mob can remember");
            configSync.AddLockingConfigEntry(_serverConfigLocked);
        }
    }
}