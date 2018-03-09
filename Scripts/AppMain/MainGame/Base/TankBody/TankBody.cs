using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZF.MainGame.Base
{
    public enum TankPart
    {
        Turret = 0,
        Bottom = 1,
        Center = 2,
        Wheel = 3,
    }

    [System.Serializable]
    public class TankBodyPart
    {
        public Configs.TankBodyPartConfig config;
        private int currentHealth = -1;

        public void Init()
        {
            currentHealth = config.health;
        }
        public int TakeDamage(int damage)
        {
            int lastHealth = currentHealth;
            currentHealth -= (int)(damage / config.protection);
            if(currentHealth < 0)
            {
                currentHealth = 0;
            }
            return lastHealth - currentHealth;
        }
        public int GetCurrentHealth()
        {
            return currentHealth;
        }
    }
    public class TankBody : MonoBehaviour
    {
        public Configs.TankBodyConfig config;
        public TankBodyPart[] Parts;
        public int tankHealth = -1;
        public void Init()
        {
            tankHealth = config.tankHealth;
            foreach(TankBodyPart part in Parts)
            {
                part.Init();
            }
        }
        public int Hit(TankPart part, int damage, out bool isKilled)
        {
            if(tankHealth == 0)
            {
                isKilled = false;
                return 0;
            }
            int lastHealth = tankHealth;
            int loss = (int) (Parts[(int)part].TakeDamage(damage / config.tankProtection) * Parts[(int)part].config.ratio);
            tankHealth -= loss;
            isKilled = false;
            if(tankHealth < 0)
            {
                isKilled = true;
                tankHealth = 0;
            }
            GetComponent<Tank>().UpdateTankInfo();
            return lastHealth - tankHealth;
        }
        public int[] GetHealthInfo()
        {
            int[] HealthInfo = new int[Parts.Length + 1];
            for(int i = 0; i< Parts.Length; i++)
            {
                HealthInfo[i] = Parts[i].GetCurrentHealth();
            }
            HealthInfo[Parts.Length] = tankHealth;
            return HealthInfo;
        }
    }
}