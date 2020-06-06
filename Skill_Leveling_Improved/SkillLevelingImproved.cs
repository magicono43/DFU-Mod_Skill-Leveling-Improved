// Project:         SkillLevelingImproved mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2019 JayH
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Last Edited: 	6/3/2020, 9:30 AM
// Modifier:		

using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine;

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

            GameManager.Instance.PlayerEntity = new MyPlayerEntityClass();

            Debug.Log("Finished mod init: SkillLevelingImproved");
        }

        public PlayerEntity playerEntity
        {
            get { return (playerEntity != null) ? playerEntity : playerEntity = PlayerEntityBehaviour.Entity as PlayerEntity; }
            set { playerEntity = value; }
        }

        #region Stuff Here



        #endregion
    }
}