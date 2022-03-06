﻿using System;
using UnityEngine;

namespace NPC_Generator.MonoScripts
{
    public class HairSetter : MonoBehaviour
    {
        private Humanoid? _hu;
        private VisEquipment? _visEquipment;
        [SerializeField]internal string? HairStyleName;

        private void Awake()
        {
            _hu = GetComponent<Humanoid>();
            _visEquipment = GetComponent<VisEquipment>();
            InvokeRepeating(nameof(SetHairStyle), 0,1);
        }

        private void SetHairStyle()
        {
            if (Player.m_localPlayer == null) return;
            _hu!.SetHair(HairStyleName);
            _hu.SetupEquipment();
            _visEquipment.SetHairItem(HairStyleName);
            _visEquipment.SetHairEquiped(HairStyleName.GetStableHashCode());
            CancelInvoke(nameof(SetHairStyle));
        }
        
    }
}