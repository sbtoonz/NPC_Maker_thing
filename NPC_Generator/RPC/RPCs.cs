using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace NPC_Generator.RPC;

public static class RPCs
{
    internal static Vector3 position = new Vector3(0f, 0f, 0f);
    private static RandomEvent _event = new RandomEvent();
    public static RandomEvent? randomEvent { get; set; }
    
    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    public static class AddRPCPatch
    {
        
        public static void Postfix()
        {
            ZRoutedRpc.instance.Register<Vector3>("RPC_Find_Location", FindLocation);
            ZRoutedRpc.instance.Register<float, float, float>("RPC_Register_Location", RegisterLocation);
            ZRoutedRpc.instance.Register<Vector3>("RPC_Villager_Raid", RegisterRandEvent);
            ZRoutedRpc.instance.Register<string, bool>("RPC_Set_Raid_Client", SetLocalEvent);
        }

       
    }
    private static void RegisterLocation(long uid, float x, float y, float z)
    {
        NPC_Generator.DebugLog(NPC_Generator.DebugLevel.All, "Recieved: "+x.ToString()+" - " + y.ToString() +" - " + z.ToString() );
        position = new Vector3(x, y, z);
        NPC_Generator.DebugLog(NPC_Generator.DebugLevel.All, position.ToString());
    }
    private static void FindLocation(long uid, Vector3 currentPlayerPos)
    {
        if (!ZNet.instance.IsServer()) return;
        var locations = ZoneSystem.instance.GetLocationList();
        foreach(var loc in locations)
        {

            float dist = Vector3.Distance(loc.m_position, currentPlayerPos);
            if(dist < 100)
            {
                if(loc.m_location.m_prefabName.Contains("House"))
                {
                    Vector3 temploc = Vector3.zero;
                    temploc = loc.m_position;
                    position = temploc;
                    NPC_Generator.DebugLog(NPC_Generator.DebugLevel.All,loc.m_position.ToString());
                    ZRoutedRpc.instance.InvokeRoutedRPC(uid, "RPC_Register_Location", temploc.x, temploc.y, temploc.z);
                    break;
                }
            }
        };
    }

    private static void RegisterRandEvent(long uid, Vector3 location)
    {
        if (!ZNet.instance.IsServer()) return;
        ZLog.LogWarning(location.ToString());
        _event.m_enabled = true;
        _event.m_biome = Heightmap.Biome.Meadows;
        _event.m_duration = 60;
        _event.m_name = "villager";
        _event.m_pos = location;
        _event.m_random = false;
        _event.m_startMessage = "The villagers are being raided!";
        _event.m_spawn = new List<SpawnSystem.SpawnData>
        {
            new SpawnSystem.SpawnData
            {
                m_biome = Heightmap.Biome.Plains,
                m_enabled = true,
                m_name = "villager",
                m_prefab = ZNetScene.instance.GetPrefab("VillageRaider"),
                m_huntPlayer = true,
                m_inForest = true,
                m_outsideForest = true,
                m_spawnAtDay = true,
                m_spawnAtNight = true,
                m_maxSpawned = 10,
                m_minLevel = 0,
                m_groupSizeMax = 10,
                m_groupSizeMin = 1,
                m_biomeArea = Heightmap.BiomeArea.Everything,
            },
            new SpawnSystem.SpawnData
            {
                m_biome = Heightmap.Biome.Meadows,
                m_enabled = true,
                m_name = "villager",
                m_prefab = ZNetScene.instance.GetPrefab("VillageRaider"),
                m_huntPlayer = true,
                m_inForest = true,
                m_outsideForest = true,
                m_spawnAtDay = true,
                m_spawnAtNight = true,
                m_maxSpawned = 10,
                m_minLevel = 0,
                m_groupSizeMax = 10,
                m_groupSizeMin = 1,
                m_biomeArea = Heightmap.BiomeArea.Everything,
            },
            new SpawnSystem.SpawnData
            {
                m_biome = Heightmap.Biome.None,
                m_enabled = true,
                m_name = "villager",
                m_prefab = ZNetScene.instance.GetPrefab("VillageRaider"),
                m_huntPlayer = true,
                m_inForest = true,
                m_outsideForest = true,
                m_spawnAtDay = true,
                m_spawnAtNight = true,
                m_maxSpawned = 10,
                m_minLevel = 0,
                m_groupSizeMax = 10,
                m_groupSizeMin = 1,
                m_biomeArea = Heightmap.BiomeArea.Everything,
            },
            new SpawnSystem.SpawnData
            {
                m_biome = Heightmap.Biome.Swamp,
                m_enabled = true,
                m_name = "villager",
                m_prefab = ZNetScene.instance.GetPrefab("VillageRaider"),
                m_huntPlayer = true,
                m_inForest = true,
                m_outsideForest = true,
                m_spawnAtDay = true,
                m_spawnAtNight = true,
                m_maxSpawned = 10,
                m_minLevel = 0,
                m_groupSizeMax = 10,
                m_groupSizeMin = 1,
                m_biomeArea = Heightmap.BiomeArea.Everything,
            },
            new SpawnSystem.SpawnData
            {
                m_biome = Heightmap.Biome.Mountain,
                m_enabled = true,
                m_name = "villager",
                m_prefab = ZNetScene.instance.GetPrefab("VillageRaider"),
                m_huntPlayer = true,
                m_inForest = true,
                m_outsideForest = true,
                m_spawnAtDay = true,
                m_spawnAtNight = true,
                m_maxSpawned = 10,
                m_minLevel = 0,
                m_groupSizeMax = 10,
                m_groupSizeMin = 1,
                m_biomeArea = Heightmap.BiomeArea.Everything,
            },
        };
        if (RandEventSystem.instance.m_events.Contains(_event))
        {
            RandEventSystem.instance.SetRandomEventByName("villager", location);
            return;
        }
        RandEventSystem.instance.m_events.Add(_event);
        RandEventSystem.instance.SetRandomEventByName("villager", location);
        ZRoutedRpc.instance.InvokeRoutedRPC("RPC_Set_Raid_Client", _event.m_name, _event.m_active);
    }

    private static void SetLocalEvent(long uid, string eventname, bool active)
    {
        _event.m_name = eventname;
        _event.m_active = active;
    }


    [HarmonyPatch(typeof(RandEventSystem), nameof(RandEventSystem.FixedUpdate))]
    private static class RandPatch
    {
        public static void Postfix(RandEventSystem __instance)
        {
            if (__instance.GetActiveEvent() == null) return;
            if (!__instance.HaveEvent(_event.m_name)) return;
            if (__instance.GetActiveEvent().m_name == _event.m_name)
            {
                randomEvent = __instance.GetActiveEvent();
            }
        }
        
    }
}