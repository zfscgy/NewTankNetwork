using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.Configs
{

    [System.Serializable]
    public class TankConfig
    {
        //Gun
        public float gunYSpeed;
        public float gunXSpeed;
        public float gunXMax;
        public float gunXMin;
        //Motion
        public float torqueMax;
        public float torqueIncreasingSpeed;
        public float steerMax;
        public float speedMax;
        public float autoParkingSpeed;
        public float autoParkingRate;
        public float autoParkingBrakeLag;
    }

    [System.Serializable]
    public class HealthConfig
    {
        public float totalHealth = 100f;
        public float protectionValue = 1f;
    }

    [System.Serializable]
    public class PartHealthConfig
    {
        public float totalHealth = 100f;
        public float protectionValue = 1f;
        public float ratioToTotalHealth = 0.1f;
    }


}
