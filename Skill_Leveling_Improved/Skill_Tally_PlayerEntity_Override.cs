using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using UnityEngine;
using System;

namespace SkillLevelingImproved
{
    public class SkillTallyOverride : PlayerEntity
    {
        #region Constructors

        public SkillTallyOverride(DaggerfallEntityBehaviour entityBehaviour)
            : base(entityBehaviour)
        {

        }

        #endregion

        #region Public Methods

        public override void TallySkill(DFCareer.Skills skill, short amount)
        {
            int skillId = (int)skill;

            try
            {
                if(skillId == 3)
                {
                    Debug.Log("You just jumped! You idiot!");
                }

                if (skillId == 0)
                {
                    Debug.Log("You just slept for an hour! You coward!");
                }

                if (skillId == 18)
                {
                    Debug.Log("You just climbed! You monkey!");
                }

                if (skillId != 27)
                {
                    Debug.Log("You just did something other than Mysticism! You donkey!");
                }

                skillUses[skillId] += amount;
                if (skillUses[skillId] > 20000)
                    skillUses[skillId] = 20000;
                else if (skillUses[skillId] < 0)
                {
                    skillUses[skillId] = 0;
                }
            }
            catch (Exception ex)
            {
                string error = string.Format("Caught exception {0} with skillId {1}.", ex.Message, skillId);

                if (skillUses == null || skillUses.Length == 0)
                    error += " skillUses is null or empty.";

                Debug.Log(error);
            }
        }

        #endregion
    }
}