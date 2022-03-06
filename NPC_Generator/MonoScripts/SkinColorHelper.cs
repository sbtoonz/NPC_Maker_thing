using System;
using UnityEngine;

namespace NPC_Generator.MonoScripts
{
    public class SkinColorHelper : MonoBehaviour
    {
        private VisEquipment _visEquipment;
        internal Color _skinColor;
        private void Awake()
        {
            _visEquipment = GetComponent<VisEquipment>();
            InvokeRepeating(nameof(setSkinColor),0,1);
        }
        
        private void setSkinColor()
        {
            if (Player.m_localPlayer == null) return;
            _visEquipment.SetSkinColor(new Vector3(_skinColor.r, _skinColor.g, _skinColor.b));
            CancelInvoke(nameof(setSkinColor));
        }
    }
}