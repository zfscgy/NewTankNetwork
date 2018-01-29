using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZF.MainGame.Base
{
    using ZF.Configs;
    using ZF.Communication;
    public class ServerCameraController : MonoBehaviour
    {
        public InputManager serverInputManager;
        public ServerCameraConfig config;
        #region Monobehavior.Callbacks
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float angVelocityY = serverInputManager.GetMouseMotion().x * config.rotationSpeed * Global.GameSettings.MouseSensitivty * Time.deltaTime;
            float angVelocityX = -serverInputManager.GetMouseMotion().y * config.rotationSpeed * Global.GameSettings.MouseSensitivty * Time.deltaTime;
            transform.eulerAngles += new Vector3(angVelocityX, angVelocityY, 0);
            transform.position += - config.movingSpeed * serverInputManager.GetAD() * Time.deltaTime * transform.right +
                config.movingSpeed * serverInputManager.GetWS() * Time.deltaTime * transform.forward;
        }
        #endregion
        public void Init(InputManager _inputManager)
        {
            serverInputManager = _inputManager;
        }
    }
}
