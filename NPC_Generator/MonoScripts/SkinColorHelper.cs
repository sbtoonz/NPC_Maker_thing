using UnityEngine;

namespace NPC_Generator.MonoScripts
{
    public class SkinColorHelper : MonoBehaviour
    {
        private VisEquipment? _visEquipment;
        [SerializeField]internal Color SkinColor;
        private void Awake()
        {
            _visEquipment = GetComponent<VisEquipment>();
            InvokeRepeating(nameof(SetSkinColor),0,1);
        }
        
        private void SetSkinColor()
        {
            if (Player.m_localPlayer == null) return;
            _visEquipment!.SetSkinColor(new Vector3(SkinColor.r * 1.1f, SkinColor.g* 1.1f, SkinColor.b* 1.1f));
            CancelInvoke(nameof(SetSkinColor));
        }
    }
}