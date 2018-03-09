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
        public float rotateSpeed_1 = 200f;
        public float rotateSpeed_2 = 150f;
        //Angle of pitch
        public float directionVMax = 60f;
        public float directionVMin = -30f;
        //Field of view
        public float fov_1 = 65f;
        public float fov_2 = 10f;
        //Zooming Speed
        public float zoomSpeed = 15f;
    }
    [System.Serializable]
    public class ServerCameraConfig
    {
        public float fov_1 = 65f;
        public float movingSpeed = 10f;
        public float rotationSpeed = 20f;
    }
}
