namespace NPC_Generator.MonoScripts
{
    public class Commandable : Tameable 
    {
        public new void Awake()
        {
            base.Awake();
            m_commandable = true;
        }


        public new bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (m_commandable)
            {
                Command(user);
            }
            return false;
        }
        
        public new void OnConsumedItem(ItemDrop item)
        {
            return;
        }

        public new bool IsHungry()
        {
            return false;
        }
        public new string GetStatusString()
        {
            if (m_monsterAI.IsAlerted())
            {
                return "$hud_tamefrightened";
            }
            return "$hud_tameinprogress";
        }

    }
}