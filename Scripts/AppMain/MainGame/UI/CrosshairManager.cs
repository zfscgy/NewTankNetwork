using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ZF.MainGame.UI
{
    using ZF.Configs;
    using ZF.MainGame.Base;
    public class CrosshairManager : MonoBehaviour
    {
        private TankComponents tankComponents;
        private Camera mainCamera;
        public Image actualPoint;
        #region Monobehavior Callbacks
        private void Start()
        {
            if(Global.GameState.mode == Global.GameMode.isServer)
            {
                actualPoint.gameObject.SetActive(false);
                this.enabled = false;
            }
        }
        private void Update()
        {
            actualPoint.rectTransform.position = mainCamera.WorldToScreenPoint(tankComponents.GetTurretPointing());
        }
        #endregion
        public void Init(TankComponents _tankComponents, Camera _mainCamera)
        {
            tankComponents = _tankComponents;
            mainCamera = _mainCamera;
        }
    }
}
