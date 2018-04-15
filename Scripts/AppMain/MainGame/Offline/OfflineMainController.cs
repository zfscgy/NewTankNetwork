using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.MainGame.Offline
{
    using Communication;
    using Base;
    using Global;
    public class OfflineMainController:MonoBehaviour,IMainMaster
    {
        public BirthPoints birthPoints;
        public GameObject playerPrefab;
        public GameObject cameraPrefab;
        public InstructionManager instructionManager;
        public InputManager inputManager;
        public InstructionSender instructionSender;
        public Stats.GameStatManager statManager;
        public UI.CrosshairManager crosshairManager;
        public Server.ServerUIController serverUIController;
        public Environment.SensoringSimulator sensoringSimulator;
        private CameraController playerCamera;
        private Tank playerTank;
        private Tank[] Players = Singletons.gameRoutineController.GetTanks();
        public void Init()
        {
            enabled = true;
            GameState.playerNum = 1;
            GameState.playerID = 0;
            playerCamera = Instantiate(cameraPrefab).GetComponent<Base.CameraController>();
            playerTank = Instantiate(playerPrefab, birthPoints.points[0].position, birthPoints.points[0].rotation).GetComponentInChildren<Tank>();
            Players[0] = playerTank;
            playerTank.InitOnOfflineGame(0, instructionManager.GetInstruction());
            playerCamera.Init(inputManager, playerTank.transform);
            instructionManager.SetCamera(playerCamera);
            crosshairManager.Init(playerTank.motion.tankComponents, playerCamera.GetComponentInChildren<Camera>());
            statManager.Init(Players);
            playerTank.UpdateTankInfo();
            serverUIController.Init();
            sensoringSimulator.Init(Players);
        }

        bool IMainMaster.DeleteBot(Tank aiTank)
        {
            throw new NotImplementedException();
        }

        bool IMainMaster.SetBot(Tank aiTank)
        {
            if(GameState.playerNum == Global.playerPerRoom)
            {
                return false;
            }
            aiTank.transform.position = birthPoints.points[GameState.playerNum].position;
            aiTank.transform.rotation = birthPoints.points[GameState.playerNum].rotation;
            Players[GameState.playerNum] = aiTank;
            aiTank.InitAI(GameState.playerNum, false);
            GameState.playerNum++;
            return true;
        }
    }
}
