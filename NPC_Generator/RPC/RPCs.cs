using HarmonyLib;
using UnityEngine;

namespace NPC_Generator.RPC;

public class RPCs
{
    internal static Vector3 position = new Vector3(0f, 0f, 0f);
    
    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    public static class AddRPCPatch
    {
        
        public static void Postfix()
        {
            ZRoutedRpc.instance.Register<Vector3>("RPC_Find_Location", FindLocation);
            ZRoutedRpc.instance.Register<float, float, float>("RPC_Register_Location", RegisterLocation);
        }

        private static void RegisterLocation(long uid, float x, float y, float z)
        {
            NPC_Generator.DebugLog(NPC_Generator.DebugLevel.All, "Recieved: "+x.ToString()+" - " + y.ToString() +" - " + z.ToString() );
            position = new Vector3(x, y, z);
            NPC_Generator.DebugLog(NPC_Generator.DebugLevel.All, position.ToString());
        }
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
}