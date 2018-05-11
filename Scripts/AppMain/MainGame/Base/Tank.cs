
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.MainGame.Base
{
    using Communication;
    using Weapon;
    using AI;
    public class Tank : MonoBehaviour
    {
        public int seatID;
        public TankBody body;
        public TankWeapon weapon;
        public TankMotion motion;
        public TankAIController AIController;

        public Texts.Info3dText textDisplayer;
        private bool isInitialized = false;
        private Color[] Colors = new Color[] { Color.red, Color.blue };
        public void InitSyncMode(int _seatID, Syncer syncer)
        {
            if (isInitialized)
            {
                return;
            }
            seatID = _seatID;
            body.enabled = false;
            weapon.enabled = false;
            motion.SetMode(TankMode.Sync);
            motion.tankNetworkComponents.Set(syncer);
            isInitialized = true;
            if(seatID != Global.GameState.playerID)
            {
                textDisplayer.Init(Colors[2 * seatID / Global.Global.playerPerRoom], seatID.ToString());
            }
            else
            {
                textDisplayer.gameObject.SetActive(false);
            }
            UpdateTankInfo();
        }
        public void InitOnServer(int _seatID, Instruction instruction, Syncer syncer)
        {
            if (isInitialized)
            {
                return;
            }
            seatID = _seatID;
            body.Init();
            motion.SetMode(TankMode.Server);
            motion.SetInstruction(instruction);
            motion.tankNetworkComponents.Set(syncer);
            weapon.Init(instruction);
            isInitialized = true;
            if (seatID != Global.GameState.playerID)
            {
                textDisplayer.Init(Colors[2 * seatID / Global.Global.playerPerRoom], seatID.ToString());
            }
            else
            {
                textDisplayer.gameObject.SetActive(false);
            }
            UpdateTankInfo();
        }
        public void InitOnOfflineGame(int _seatID, Instruction instruction)
        {
            if (isInitialized)
            {
                return;
            }
            seatID = _seatID;
            body.Init();
            motion.SetMode(TankMode.Local);
            motion.SetInstruction(instruction);
            weapon.Init(instruction);
            isInitialized = true;
            if (seatID != Global.GameState.playerID)
            {
                textDisplayer.Init(Colors[2 * seatID / Global.Global.playerPerRoom], seatID.ToString());
            }
            else
            {
                textDisplayer.gameObject.SetActive(false);
            }
            UpdateTankInfo();
        }
        public void InitAI(int _seatID, bool isOnServer, Syncer syncer = null)
        {
            if (isInitialized)
            {
                return;
            }
            seatID = _seatID;
            body.Init();
            motion.SetMode(isOnServer ? TankMode.Server : TankMode.Local);
            if(isOnServer)
            {
                motion.tankNetworkComponents.Set(syncer);
            }
            AIController.Init(TankAIState.waiting);
            isInitialized = true;
            if (seatID != Global.GameState.playerID)
            {
                textDisplayer.Init(Colors[2 * seatID / Global.Global.playerPerRoom], seatID.ToString());
            }
            else
            {
                textDisplayer.gameObject.SetActive(false);
            }
            UpdateTankInfo();
        }

        /*
         * Get informations about tank.
         */

        private Stats.TankStat stat = new Stats.TankStat();
        public Stats.TankStat GetStat() { return stat; }
        public void UpdateTankInfo()
        {
            stat.health = body.tankHealth;
            for(int i = 0;i < Global.GameSettings.n_TankBodyPart; i ++)
            {
                stat.PartHealth[i] = body.Parts[i].GetCurrentHealth();
            }
            stat.output = weapon.GetTotalOutput();
            stat.hit = weapon.GetTotalHit();
            stat.kill = weapon.GetTotalKill();
            for(int i = 0;i < Global.GameSettings.n_AmmoKind; i++)
            {
                stat.AmmoRemain[i] = weapon.Ammos[i].GetCurrentNumber();
            }
            if (Global.GameState.mode == Global.GameMode.isServer)
            {
                Global.GameState.gameStatManager.CallClientToUpdateStat(seatID);
            }
            else if (Global.GameState.mode == Global.GameMode.inOfflineGame)
            {
                Global.GameState.gameStatManager.CallUIToUpdate();
            }
        }
        public void UpdateSpeedInfo()
        {
            stat.speed = (int)motion.GetSpeed();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Die()
        {
            motion.enabled = false;
            weapon.enabled = false;
            if (AIController != null)
            {
                AIController.StopAI();
            }
        }


        private void Update()
        {
            UpdateSpeedInfo();
        }
    }
}
