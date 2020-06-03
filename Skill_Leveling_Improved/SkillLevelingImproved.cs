// Project:         SkillLevelingImproved mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2019 JayH
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Last Edited: 	6/3/2020, 9:30 AM
// Modifier:		

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
    public class SkillLevelingImproved : MonoBehaviour
    {
        static Mod mod;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject("SkillLevelingImproved");
            go.AddComponent<SkillLevelingImproved>();
        }

        void Awake()
        {
            InitMod();

            mod.IsReady = true;
        }

        private static void InitMod()
        {
            Debug.Log("Begin mod init: SkillLevelingImproved");
			
			FormulaHelper.RegisterOverride(mod, "CalculateHandToHandMinDamage", (Func<int, int>)CalculateHandToHandMinDamage);
			FormulaHelper.RegisterOverride(mod, "CalculateHandToHandMaxDamage", (Func<int, int>)CalculateHandToHandMaxDamage);
			FormulaHelper.RegisterOverride(mod, "CalculateStruckBodyPart", (Func<int>)CalculateStruckBodyPart);
			FormulaHelper.RegisterOverride(mod, "CalculateBackstabChance", (Func<PlayerEntity, DaggerfallEntity, int, int>)CalculateBackstabChance);
			FormulaHelper.RegisterOverride(mod, "CalculateBackstabDamage", (Func<int, int, int>)CalculateBackstabDamage);
			FormulaHelper.RegisterOverride(mod, "GetBonusOrPenaltyByEnemyType", (Func<DaggerfallEntity, EnemyEntity, int>)GetBonusOrPenaltyByEnemyType);
			FormulaHelper.RegisterOverride(mod, "GetMeleeWeaponAnimTime", (Func<PlayerEntity, WeaponTypes, ItemHands, float>)GetMeleeWeaponAnimTime);
			FormulaHelper.RegisterOverride(mod, "GetBowCooldownTime", (Func<PlayerEntity, float>)GetBowCooldownTime);
			
			Debug.Log("Finished mod init: SkillLevelingImproved");
		}
		
		#region Physical Combat Formulas
		
		private static int CalculateHandToHandMinDamage(int handToHandSkill) // Later on, possibly add racial mods to this, such as giving khajiit a bonus or something. // !!!!Needs Alteration
		{
			return (handToHandSkill / 10) + 1;
		}
		
		private static int CalculateHandToHandMaxDamage(int handToHandSkill) // Later on, possibly add racial mods to this, such as giving khajiit a bonus or something. // !!!!Needs Alteration
		{
			return (handToHandSkill / 5) + 1;
		}
		
		private static int CalculateStruckBodyPart() // Possibly change so that all body parts have an equal chance of being hit, or something around that. // !!!!Needs Alteration
		{
			int[] bodyParts = { 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6 };
            return bodyParts[UnityEngine.Random.Range(0, bodyParts.Length)];
		}
		
		private static int CalculateBackstabChance(PlayerEntity player, DaggerfallEntity target, int enemyAnimStateRecord) // !!!!Needs Alteration
        {
            // If enemy is facing away from player
            if (enemyAnimStateRecord % 5 > 2)
            {
                player.TallySkill(DFCareer.Skills.Backstabbing, 1);
                return player.Skills.GetLiveSkillValue(DFCareer.Skills.Backstabbing);
            }
            else
                return 0;
        }
		
		private static int CalculateBackstabDamage(int damage, int backstabbingLevel) // !!!!Needs Alteration
        {
            if (backstabbingLevel > 1 && Dice100.SuccessRoll(backstabbingLevel))
            {
                damage *= 3;
                string backstabMessage = HardStrings.successfulBackstab;
                DaggerfallUI.Instance.PopupMessage(backstabMessage);
            }
            return damage;
        }
		
		static int GetBonusOrPenaltyByEnemyType(DaggerfallEntity attacker, EnemyEntity AITarget) // Could add additional racial bonuses or something. // !!!!Needs Alteration
        {
            if (attacker == null || AITarget == null)
                return 0;

            int damage = 0;
            // Apply bonus or penalty by opponent type.
            // In classic this is broken and only works if the attack is done with a weapon that has the maximum number of enchantments.
            if (AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Undead)
            {
                if (((int)attacker.Career.UndeadAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += attacker.Level;
                }
                if (((int)attacker.Career.UndeadAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= attacker.Level;
                }
            }
            else if (AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Daedra)
            {
                if (((int)attacker.Career.DaedraAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += attacker.Level;
                }
                if (((int)attacker.Career.DaedraAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= attacker.Level;
                }
            }
            else if (AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Humanoid)
            {
                if (((int)attacker.Career.HumanoidAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += attacker.Level;
                }
                if (((int)attacker.Career.HumanoidAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= attacker.Level;
                }
            }
            else if (AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Animals)
            {
                if (((int)attacker.Career.AnimalsAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += attacker.Level;
                }
                if (((int)attacker.Career.AnimalsAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= attacker.Level;
                }
            }

            return damage;
        }
		
		public static float GetMeleeWeaponAnimTime(PlayerEntity player, WeaponTypes weaponType, ItemHands weaponHands) // Not sure what this does exactly // !!!!Needs Alteration
        {
            float speed = 3 * (115 - player.Stats.LiveSpeed);
            return speed / classicFrameUpdate;
        }
		
		public static float GetBowCooldownTime(PlayerEntity player) // Not sure what this does exactly // !!!!Needs Alteration
        {
            float cooldown = 10 * (100 - player.Stats.LiveSpeed) + 800;
            return cooldown / classicFrameUpdate;
        }
		
		#endregion
		
		#region Helper Methods
		
		// Approximation of classic frame updates
        public const int classicFrameUpdate = 980;
		
		private static bool IsRingOfNamira(DaggerfallUnityItem item)
        {
            return item != null && item.ContainsEnchantment(DaggerfallConnect.FallExe.EnchantmentTypes.SpecialArtifactEffect, (int)ArtifactsSubTypes.Ring_of_Namira);
        }
		
		
		
		#endregion
    }
}

// Eventually I would like to be able to alter the paperdoll armor numbers to show reduction values, instead of the AC score values that are normally used.
// Could even add a sort of "Blocking" mechanic attached to the shields, which basically does a roll based on factors such as certain skills/attributes that would determine if the attack was fully negated or not, something like that.
// Make it where weapons of better materials will completely penetrate the resistance of lesser materials. Will have to consider this more, as well as how the math would look for either and the end results.
// Depending on how the numbers are looking by the end of this mod, I will potentially take away the hit bonus entirely from different weapon materials, and make the damage and armor penetration the main factor in what makes the higher tier weapons better, or something around those lines.
// Likely just going to make two different mods, one for "RP friendly" where material is penetrated based on the material being used, and the one i'm going to be making first which is "not so RP friendly" where each tier has a different (likely percentage based) damage reduction, similar to Oblivion and Skyrim I believe.
// This may be a different mod, but could also put into this one. Make it so armor and possibly weapon durability/current condition effects how well they function, like a worn piece of armor would reduce damage less or something and weapon do less damage. With new stuff possibly getting a increase instead.
