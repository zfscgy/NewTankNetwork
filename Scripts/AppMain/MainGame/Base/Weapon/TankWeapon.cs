using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ZF.MainGame.Base.Weapon
{
    using Communication;
    using Configs;
    public class TankWeapon:MonoBehaviour
    { 
        public WeaponConfig config;
        public WeaponComponents components;
        public Ammo[] Ammos;
        private Instruction instruction;
        private float lastFireTime;
        private int currentWeapon = 0;

        private int totalOutput = 0;
        public int GetTotalOutput() { return totalOutput; }
        private int totalHit = 0;
        public int GetTotalHit() { return totalHit; }
        private int totalKill = 0;
        public int GetTotalKill() { return totalKill; }

        private Rigidbody thisRigidbody;
        private void Start()
        {
            lastFireTime = Time.time;
        }
        private void FixedUpdate()
        {
            if(instruction == null)
            {
                return;
            }
            if(instruction.GetMouseLeft() == 1)
            {
                Fire();
            }
        }

        private bool Fire()
        {
            if(Ammos[currentWeapon].Fire())
            {
                GetComponent<Tank>().UpdateTankInfo();
                GameObject shell;
                if (Global.GameState.mode != Global.GameMode.inOfflineGame && Global.GameState.mode != Global.GameMode.asOfflineAIServer)
                {
                    shell = PhotonNetwork.Instantiate(components.shellPrefab.name,
                        components.gun.position + 8f * components.gun.forward, components.gun.rotation, 0);
                    shell.GetComponent<Shell>().Init(this, Ammos[currentWeapon].config.damage,
                        GetShellInitialVelocity(), config.shellLifetime, true);
                }
                else
                {
                    shell = Instantiate(components.shellPrefab,
                        components.gun.position + 8f * components.gun.forward, components.gun.rotation);
                    shell.GetComponent<Shell>().Init(this, Ammos[currentWeapon].config.damage, 
                        GetShellInitialVelocity(), config.shellLifetime, false);
                }
                Global.Singletons.gameInteractionManager.NewShoot(GetComponent<Tank>().seatID);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Init(Communication.Instruction _instruction)
        {
            instruction = _instruction;
            if(instruction == null)
            {
                enabled = false;
            }
            foreach(Ammo ammo in Ammos)
            {
                ammo.Init();
            }
        }

        private Vector3 GetShellInitialVelocity()
        {
            if (thisRigidbody == null)
            {
                thisRigidbody = GetComponentInChildren<Rigidbody>();
            }
            Vector3 velocity = config.initialSpeed * components.gun.forward + thisRigidbody.velocity;
            return velocity;
        }
        
        public void HitResult(int damage, bool killed)
        {
            if(damage == 0)
            {
                return;
            }
            totalHit++;
            totalOutput += damage;
            if(killed)
            {
                totalKill++;
            }
            GetComponent<Tank>().UpdateTankInfo();
        }

        public byte[] GetWeaponStat()
        {

            return new byte[4];
        }
    }

    public enum HitMode
    {
        Ground = 0,
        Tank = 1,
    }
}
