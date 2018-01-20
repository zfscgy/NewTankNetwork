using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ZF.Configs
{
    [System.Serializable]
    public class CameraComponents
    {
        //Physics Components
        public Transform cameraPos;
        public Transform cameraHolder;
    }
    [System.Serializable]
    public class TankComponents
    {
        public Transform turret;
        public ZF.MainGame.Base.WheelRotate[] LeftWheels;
        public ZF.MainGame.Base.WheelRotate[] RightWheels;
    }


}
