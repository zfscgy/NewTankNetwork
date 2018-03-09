using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.MainGame.Configs
{
    [System.Serializable]
    public class TankBodyConfig
    {
        public int tankHealth;
        public int tankProtection;
    }

    [System.Serializable]
    public class TankBodyPartConfig
    {
        public float ratio;
        public int health;
        public float protection;
    }
}
