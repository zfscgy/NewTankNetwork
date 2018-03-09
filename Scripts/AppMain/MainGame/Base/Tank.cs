
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
        private bool isInitialized = false;
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
            AIController.Init();
            isInitialized = true;
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
            stat.speed = (int)(motion.m_rigidbody.velocity.magnitude * 3.6f);
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
    }
}
