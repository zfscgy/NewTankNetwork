using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ZF.MainGame.Base.Weapon
{
    using Configs;
    [System.Serializable]
    public class Ammo
    {
        public AmmoConfig config;
        private int currentNumber = -1;
        public int GetCurrentNumber() { return currentNumber; }
        private float lastFireTime;
        public void Init()
        {
            currentNumber = config.totalNumber;
        }

        public bool Fire()
        {
            if(currentNumber == -1)
            {
                currentNumber = config.totalNumber - 1;
                lastFireTime = Time.time; 
                return true;
            }
            else if(currentNumber > 0 && Time.time - lastFireTime > config.reloadTime)
            {
                currentNumber--;
                lastFireTime = Time.time;
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}
