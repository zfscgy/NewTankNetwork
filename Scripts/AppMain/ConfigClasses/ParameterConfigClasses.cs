using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.Configs
{
    [System.Serializable]
    public class CameraConfig
    {
        //Distance from the player
        public float height = 3f;
        public float distanceInit = 5f;
        public float distanceMax = 15f;
        public float distanceMin = -3f;
        //Rotate speed
        public float rotateSpeed_1 = 20f;
        public float rotateSpeed_2 = 15f;
        //Angle of pitch
        public float directionVMax = 0f;
        public float directionVMin = -90f;
        //Field of view
        public float fov_1 = 65f;
        public float fov_2 = 10f;
        //Zooming Speed
        public float zoomSpeed = 15f;
    }
    [System.Serializable]
    public class TankConfig
    {
        //Shooting
        public float shootInterval;
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

    [System.Serializable]
    public class ServerCameraConfig
    {
        public float fov_1 = 65f;
        public float movingSpeed = 10f;
        public float rotationSpeed = 20f;
    }
}
