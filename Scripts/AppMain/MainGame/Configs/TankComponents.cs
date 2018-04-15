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
        public Transform cameraBase;
        public Transform cameraHolder;
        public Transform camera;
    }
    [System.Serializable]
    public class TankComponents
    {
        public Transform turret;
        public Transform gun;
        public MainGame.Base.WheelRotate[] LeftWheels;
        public MainGame.Base.WheelRotate[] RightWheels;
        public GameObject deadEffect;
        public Vector3 GetTurretPointing()
        {
            Ray turretRay = new Ray(gun.position + 3 * gun.forward, gun.forward);
            RaycastHit hitPoint;
            if (Physics.Raycast(turretRay, out hitPoint, Global.GameSettings.maxRayDistance))
            {
                return hitPoint.point;
            }
            else
            {
                return gun.position + Global.GameSettings.maxRayDistance * gun.forward;
            }
        }
    }


}
