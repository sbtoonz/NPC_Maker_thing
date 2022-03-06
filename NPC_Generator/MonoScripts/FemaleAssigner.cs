using UnityEngine;

namespace NPC_Generator.MonoScripts
{
    public class FemaleAssigner : MonoBehaviour
    {
        private VisEquipment? _visEquipment;

        private void OnEnable()
        {
            _visEquipment = GetComponent<VisEquipment>();
            InvokeRepeating(nameof(SetFemaleModel), 0, 1);
        }

        private void SetFemaleModel()
        {
            if (Player.m_localPlayer == null) return;
            _visEquipment?.SetModel(1);
            CancelInvoke(nameof(SetFemaleModel));
        }
    }
}