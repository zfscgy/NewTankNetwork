using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ZF;
namespace ZF.MainGame.Client
{
    [System.Serializable]
    public class StartPoints
    {
        public Transform[] points;
    }
    public class MainGameLoader:MonoBehaviour
    {
        public StartPoints[] StartPointsArray;
        public GameObject clientControllerPrefab;
        public GameObject playerPrefab;
        public GameObject cameraPrefab;
        private Communication.InstructionManager instructionManager;
        private Communication.InputManager inputManager;
        private Base.TankMotion playerTankMotion;
        private Base.CameraController playerCamera;
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GameObject clientController;
            GameObject player;
            clientController = Instantiate(clientControllerPrefab);
            inputManager = clientController.GetComponent<Communication.InputManager>();
            instructionManager = clientController.GetComponent<Communication.InstructionManager>();
            playerCamera = Instantiate(cameraPrefab).GetComponent<Base.CameraController>();
            if(ZF.Global.GameState.isOnline == false)
            {
                player = Instantiate(playerPrefab, StartPointsArray[0].points[0].position, StartPointsArray[0].points[0].rotation);
                playerTankMotion = player.GetComponentInChildren<Base.TankMotion>();
                playerTankMotion.SetInstruction(instructionManager.GetInstruction());
                playerTankMotion.SetMode(Base.TankMode.Local);
                playerCamera.Init(inputManager,playerTankMotion.transform);
            }

        }
    }
}
