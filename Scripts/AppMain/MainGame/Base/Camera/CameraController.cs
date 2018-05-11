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
        public Camera mainCamera;

        private void Start()
        {
        }
        private void FixedUpdate()
        {
            if(inputManager == null)
            {
                return;
            }
            ControlCamera();
            ControlZoom();
        }

        private void ControlCamera()
        {
            float angVelocityY = inputManager.GetMouseMotion().x * cameraConfig.rotateSpeed_1 * Global.GameSettings.MouseSensitivty * Time.fixedDeltaTime;
            float angVelocityX = - inputManager.GetMouseMotion().y * cameraConfig.rotateSpeed_1 * Global.GameSettings.MouseSensitivty * Time.fixedDeltaTime;
            cameraComponents.cameraBase.localEulerAngles += new Vector3(0, angVelocityY, 0);
            float currentAngleX = cameraComponents.cameraHolder.localEulerAngles.x + angVelocityX;
            currentAngleX = currentAngleX < 180f 
                ? currentAngleX : currentAngleX - 360f;
            float newAngleX = Mathf.Clamp(currentAngleX, -cameraConfig.directionVMax, -cameraConfig.directionVMin);
            cameraComponents.cameraHolder.localEulerAngles = new Vector3(newAngleX, cameraComponents.cameraHolder.localEulerAngles.y, 0f);
        }

        private float cameraDistance = 8f;
        private bool isTelescope = false;
        private Vector3 lastPosition;
        private float lastMouseTime = -10f;
        private void ControlZoom()
        {
            float zoom = inputManager.GetScroll() * cameraConfig.zoomSpeed * Time.fixedDeltaTime;
            cameraDistance = Mathf.Clamp(cameraDistance + zoom, cameraConfig.distanceMin, cameraConfig.distanceMax);
            cameraComponents.camera.localPosition = -cameraDistance * Vector3.forward;
            if (inputManager.GetMouseRight() && Time.time - lastMouseTime > 0.5f)
            {
                lastMouseTime = Time.time;
                if (!isTelescope)
                {
                    mainCamera.fieldOfView = cameraConfig.fov_2;
                    lastPosition = cameraComponents.camera.localPosition;
                    cameraComponents.camera.localPosition = new Vector3(0f,0f,0f);
                }
                else
                {
                    cameraComponents.camera.localPosition = lastPosition;
                    mainCamera.fieldOfView = cameraConfig.fov_1;
                }
                isTelescope = !isTelescope;
            }
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
            Ray cameraRay = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hitPoint;
            if (Physics.Raycast(cameraRay, out hitPoint, 400.0f, LayerMask.GetMask("Ground", "Tank")))
            {
                return hitPoint.point;
            }
            else
            {
                return transform.position + 400.0f * cameraComponents.cameraHolder.forward;
            }
        }

    }
}
