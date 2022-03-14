using System;
using UnityEngine;

namespace NPC_Generator.MonoScripts;

public class CapsuleHelper : MonoBehaviour
{
    public CapsuleCollider capCollider;
    private void Awake()
    {
        capCollider = GetComponent<CapsuleCollider>();
        capCollider.enabled = true;
    }
}