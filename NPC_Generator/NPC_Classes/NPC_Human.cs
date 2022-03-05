using System;
using UnityEngine;

namespace NPC_Generator.NPC_Classes
{
    public class NPC_Human : MonoBehaviour
    {
        private void Awake()
        {
            var mai = gameObject.GetComponent<MonsterAI>();
            mai.m_alertedEffects.m_effectPrefabs = new EffectList.EffectData[0];
            mai.m_idleSound.m_effectPrefabs = new EffectList.EffectData[0];
            mai.m_nview = GetComponent<ZNetView>();
            
            mai.m_nview.GetZDO().SetPrefab(NPC_Generator.NetworkedNPC.GetHashCode());
                ZNetScene.instance.GetPrefabHash(ZNetScene.instance.GetPrefab("BasicHuman"));
        }
    }
}