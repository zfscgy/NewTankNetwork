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
        public MainGameUI mainGameUI;
        public Environment.SensoringSimulator sensoringSimulator;
        private CameraController playerCamera;
        private Tank playerTank;
        private Tank[] Players = Singletons.gameRoutineController.GetTanks();
        public void Init()
        {
            enabled = true;
            GameState.nPlayer = 1;
            GameState.playerID = 0;
            statManager.Init(Players);
            Singletons.gameRoutineController.RegisterNewTank();
            playerCamera = Instantiate(cameraPrefab).GetComponent<Base.CameraController>();
            playerTank = Instantiate(playerPrefab, birthPoints.points[0].position, birthPoints.points[0].rotation).GetComponentInChildren<Tank>();
            Players[0] = playerTank;
            playerTank.InitOnOfflineGame(0, instructionManager.GetInstruction());
            playerCamera.Init(inputManager, playerTank.transform);
            instructionManager.SetCamera(playerCamera);
            crosshairManager.Init(playerTank.motion.tankComponents, playerCamera.GetComponentInChildren<Camera>());
            statManager.AddTank(playerTank);
            mainGameUI.Init(statManager.GetAllStats()[0]);
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
            int seatID = Singletons.gameRoutineController.RegisterNewTank();
            if(seatID == -1)
            {
                return false;
            }
            aiTank.transform.position = birthPoints.points[seatID].position;
            aiTank.transform.rotation = birthPoints.points[seatID].rotation;
            Players[seatID] = aiTank;
            aiTank.InitAI(seatID, false);
            return true;
        }
        
        bool IMainMaster.SetBot(Tank aiTank, int id)
        {
            int seatID = Singletons.gameRoutineController.RegisterNewTank(id);
            if (seatID == -1)
            {
                return false;
            }
            aiTank.transform.position = birthPoints.points[seatID].position;
            aiTank.transform.rotation = birthPoints.points[seatID].rotation;
            Players[seatID] = aiTank;
            aiTank.InitAI(seatID, false);
            return true;
        }
    }
}
