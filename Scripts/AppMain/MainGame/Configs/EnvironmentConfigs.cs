using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.MainGame.Configs
{
    [System.Serializable]
    public class EnvironmentConfigs
    {
        public float tankTransparityDecreasing = 0.9f;
        public float tankTransparityIncreasingByShooting = 4f;
        public float tankTransparityIncreasingByHit = 3f;

    }
}
