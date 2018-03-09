using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ZF.MainGame.Configs
{
    [System.Serializable]
    public class AmmoConfig
    {
        public float reloadTime;
        public int totalNumber;
        public int damage;
    }
    [System.Serializable]
    public class WeaponConfig
    {
        public int ammoNumber;
        public float initialSpeed;
        public float reloadTime;
        public float shellLifetime;
    }

}
