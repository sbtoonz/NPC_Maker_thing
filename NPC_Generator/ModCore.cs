using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace NPC_Generator
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class NPC_Generator : BaseUnityPlugin
    {
        private const string ModName = "NPC_Maker";
        private const string ModVersion = "1.0";
        private const string ModGUID = "com.odinplus.NPC_Maker";
        private static Harmony harmony = null!;
        internal static GameObject? RootGOHolder;
        internal static GameObject? NetworkedNPC;
        
        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            harmony = new(ModGUID);
            harmony.PatchAll(assembly);
            RootGOHolder = new GameObject("NPC");
            DontDestroyOnLoad(RootGOHolder);
            RootGOHolder.SetActive(false);
        }
        public void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
