using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.MainGame.Base
{
    using ZF.Configs;
    public class CameraController:MonoBehaviour
    {
        public CameraConfig cameraConfig;
        public CameraComponents cameraComponents;

        private Transform followingTransform;
        private ZF.Communication.InputManager inputManager;
        private Camera mainCamera;
        private void FixedUpdate()
        {
            if(inputManager == null)
            {
                return;
            }
            ControlCamera();
        }

        private void ControlCamera()
        {
            float angVelocityY = inputManager.GetMouseMotion().x * cameraConfig.rotateSpeed_1 * Global.GameSettings.MouseSensitivty * Time.fixedDeltaTime;
            float angVelocityX = - inputManager.GetMouseMotion().y * cameraConfig.rotateSpeed_1 * Global.GameSettings.MouseSensitivty * Time.fixedDeltaTime;
            cameraComponents.cameraPos.localEulerAngles += new Vector3(0, angVelocityY, 0);
            float currentAngleX = cameraComponents.cameraHolder.localEulerAngles.x + angVelocityX;
            currentAngleX = currentAngleX < 180f 
                ? currentAngleX : currentAngleX - 360f;
            float newAngleX = Mathf.Clamp(currentAngleX, -cameraConfig.directionVMax, -cameraConfig.directionVMin);
            cameraComponents.cameraHolder.localEulerAngles = new Vector3(newAngleX, cameraComponents.cameraHolder.localEulerAngles.y, 0f);
        }

        public void Init(ZF.Communication.InputManager _inputManager, Transform _followingTransform)
        {
            inputManager = _inputManager;
            SetFollowingTransform(_followingTransform);
        }
        public void SetFollowingTransform(Transform _followingTransform)
        {
            followingTransform = _followingTransform;
            transform.parent = _followingTransform;
            transform.localEulerAngles = new Vector3();
            transform.localPosition = new Vector3(0, cameraConfig.height, 0);
        }

        public Vector3 GetCameraPointing()
        {
            return new Vector3();
        }

    }
}
