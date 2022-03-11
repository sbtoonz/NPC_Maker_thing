using HarmonyLib;
using NPC_Generator.MonoScripts;
using NPC_Generator.MonoScripts.Villagers;
using UnityEngine;

namespace NPC_Generator.RPC;

public class RPCs
{
    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    public static class AddRPCPatch
    {
        public static void Postfix()
        {
            ZRoutedRpc.instance.Register<Vector3, string>("RPC_PlaceMessenger", PlaceMessenger);
            ZRoutedRpc.instance.Register<bool, string>("RPC_Place_Response", PlaceResponse);
            ZRoutedRpc.instance.Register<string>("RPC_Find_Location", FindLocation);
        }

        private static void FindLocation(long uid, string PrefabName)
        {
            if (!ZNet.instance.IsServer()) return;
            var locations = ZoneSystem.instance.GetLocationList();
            foreach(var loc in locations)
            {

                float dist = Vector3.Distance(loc.m_position, Player.m_localPlayer.transform.position);
                if(dist < 100)
                {
                    if(loc.m_location.m_prefabName.Contains("House"))
                    {
                        ZRoutedRpc.instance.InvokeRoutedRPC("RPC_PlaceMessenger", loc.m_location, PrefabName);
                        break;
                    }
                }
            };
        }

        private static void PlaceResponse(long uid, bool isPlaced, string placedMessage)
        {
            NPC_Generator.DebugLog(NPC_Generator.DebugLevel.All,placedMessage);
        }

        private static void PlaceMessenger(long uid, Vector3 pos, string prefabName)
        {
            if (!ZNet.instance.IsServer()) return;
            var pgo = ZNetScene.instance.GetPrefab("prefabName");
            var go  = Object.Instantiate(pgo, NPC_Generator.RootGOHolder!.transform);
            var hair = go.GetComponent<HairSetter>();
            Object.Destroy(hair);
            var skin = go.GetComponent<SkinColorHelper>();
            Object.Destroy(skin);
            var femassign = go.GetComponent<FemaleAssigner>();
            Object.Destroy(femassign);
            float y;
            var messeneger = go.GetComponent<VillagerMessenger>();
            Object.Destroy(messeneger);
            go.AddComponent<RandomVisuals>();
            var target =go.AddComponent<MessengerTarget>();
            //target.m_sendernview = m_nview;
            ZoneSystem.instance.FindFloor(pos,out y);
            pos = new Vector3(pos.x,y+2,pos.z);
            go.transform.localPosition = pos;
            go.transform.SetParent(Game.instance.transform.parent);
            ZRoutedRpc.instance.InvokeRoutedRPC("RPC_Place_Response", true, "Place Quest Worker at " + pos);
        }
    }
}