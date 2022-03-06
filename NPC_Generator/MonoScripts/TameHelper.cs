using UnityEngine;

namespace NPC_Generator.MonoScripts
{
    public class TameHelper : MonoBehaviour
    {
        private MonsterAI? _monsterAI;
        private Humanoid? _humanoid;

        private void OnEnable()
        {
            _monsterAI = GetComponent<MonsterAI>();
            _humanoid = GetComponent<Humanoid>();
            InvokeRepeating(nameof(TameChecker), 0, 1f);
        }

        private void TameChecker()
        {
            if(Player.m_localPlayer == null) return;
            if (_humanoid!.IsTamed())
            {
                _monsterAI!.m_targetCreature = null;
                _monsterAI.SetAlerted(false);
                _monsterAI.m_targetStatic = null;
                _monsterAI.SetFollowTarget(null);
                _monsterAI.SetTarget(null);
                _monsterAI.UpdateTarget(null, 0, out bool test, out bool test2);
                _monsterAI.SetHuntPlayer(false);
                _humanoid.m_faction = Character.Faction.Players;
                _humanoid.m_group = Player.m_localPlayer.m_group;
                _monsterAI.m_avoidFire = false;
                _monsterAI.m_avoidLand = false;
                _monsterAI.m_avoidWater = false;
                CancelInvoke(nameof(TameChecker));
            }
        }
    }
}