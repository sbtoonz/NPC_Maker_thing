﻿using System;
using NPC_Generator.NPC_Utilities;
using UnityEngine;

namespace NPC_Generator.MonoScripts.Villagers
{
    public class VillagerBase : MonoBehaviour, Hoverable, Interactable
    {
        public string m_name;
        public GameObject m_talker;
        public Animator m_ani;
        public Humanoid hum;
        public ZNetView m_nview;
        protected readonly float QuestCD = 1800;
        
        public virtual void Awake()
        {
            m_talker = this.gameObject;
            m_ani = GetComponentInChildren<Animator>();
            m_nview = GetComponent<ZNetView>();
            hum = GetComponent<Humanoid>();
            _monsterAI = gameObject.GetComponent<MonsterAI>();
            _monsterAI!.m_targetCreature = null;
            _monsterAI.SetAlerted(false);
            _monsterAI.m_targetStatic = null;
            _monsterAI.SetFollowTarget(null);
            _monsterAI.SetTarget(null);
            _monsterAI.UpdateTarget(null, 0, out bool test, out bool test2);
            _monsterAI.SetHuntPlayer(false);
            hum.m_faction = Character.Faction.Players;
            hum.m_group = "";
            _monsterAI.m_avoidFire = false;
            _monsterAI.m_avoidLand = false;
            _monsterAI.m_avoidWater = false;
            hum.m_onDamaged = (Action<float, Character>)Delegate.Combine(hum.m_onDamaged, (Action<float, Character>)(Damage));
            NPC_Human.spawnedVillagers.Add(this);
        }
        public MonsterAI _monsterAI { get; set; }


        public virtual string GetHoverText()
        {
            return m_name;
        }

        public virtual string GetHoverName()
        {
            return m_name;
        }

        public virtual bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (hold)
            {
                return false;
            }
            Say("Greetings");
            return true;
        }

        public virtual bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return false;
        }

        public virtual void Say(string text, string emote)
        {
            if (hum.m_faction != Character.Faction.Players)
            {
                return;
            }
            text = Localization.instance.Localize(text);
            var tname = Localization.instance.Localize(m_name);
            Chat.instance.SetNpcText(m_talker, Vector3.up * 1.5f, 60f, 5, tname, text, false);
            m_ani.SetTrigger(emote);
        }
        
        public virtual void Say(string text)
        {
            text=Localization.instance.Localize(text);
            var tname=Localization.instance.Localize(m_name);
            Chat.instance.SetNpcText(m_talker, Vector3.up * 1.5f, 60f, 5, tname, text, false);
        }
        
        public virtual void Damage(float hit, Character character)
        {
            if (character == null)
            {
                return;
            }
            if (character.IsPlayer())
            {
                foreach (var item in NPC_Human.spawnedVillagers)
                {
                    item.ChangeFaction(item.hum);
                }
            }
        }
        public virtual void ChangeFaction(Humanoid m_hum)
        {
            m_hum.m_faction = Character.Faction.PlainsMonsters;
        }

        protected bool IsQuestReady()
        {
            var complete = m_nview.GetZDO().GetBool("QuestComplete");
            if (complete)
            {
                return false;
            }
            DateTime d = new DateTime(this.m_nview.GetZDO().GetLong("QuestTime", (long)QuestCD));
            bool result = (ZNet.instance.GetTime() - d).TotalSeconds > (double)QuestCD;
            return result;
        }
        public void ResetQuestCD()
        {
            m_nview.GetZDO().Set("QuestTime",ZNet.instance.GetTime().Ticks);
            m_nview.GetZDO().Set("QuestComplete", true);
        }

        protected bool IsQuestDone()
        {
            return m_nview.GetZDO().GetBool("QuestComplete");
        }
    }
}