using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ZF;
namespace ZF.MainGame.Client
{
    using Communication;
    using Base;
    public class MainGameLoader:Photon.MonoBehaviour
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
        public AI.MainGameAIController mainGameAIController;
        public Environment.SensoringSimulator sensoringSimulator;
        private Tank playerTank;
        private Tank[] Players = new Tank[Global.Global.playerPerRoom];
        private CameraController playerCamera;
        private void Awake()
        {
            Global.Singletons.wholeGameController.LoadMainGame();
        }
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if(Global.GameState.mode == Global.GameMode.isServer)
            {
                mainGameAIController.Init();
                enabled = false;
                return;
            }

            GameObject player;
            playerCamera = Instantiate(cameraPrefab).GetComponent<Base.CameraController>();
            
            if (ZF.Global.GameState.mode == Global.GameMode.inOfflineGame)
            {
                Global.GameState.playerNum = 1;
                Global.GameState.playerID = 0;
                player = Instantiate(playerPrefab, birthPoints.points[0].position, birthPoints.points[0].rotation);
                playerTank = player.GetComponentInChildren<Tank>();
                Players[0] = playerTank;
                playerTank.InitOnOfflineGame(0, instructionManager.GetInstruction());
                playerCamera.Init(inputManager,playerTank.transform);
                instructionManager.SetCamera(playerCamera);
                crosshairManager.Init(playerTank.motion.tankComponents, playerCamera.GetComponentInChildren<Camera>());
                statManager.Init(Players);
                playerTank.UpdateTankInfo();
                serverUIController.Init();
                mainGameAIController.Init();
                sensoringSimulator.Init(Players);
            }
            else
            {
                instructionSender.Init(Global.GameSettings.serverIPAddress, 
                    instructionManager.GetInstruction(), PhotonNetwork.player.ID);
                photonView.RPC("RPCOnePlayerReady", PhotonTargets.MasterClient);
            }

        }

        #region Public Methods
        public void SetBotOffline(Tank bot)
        {
            Players[Global.GameState.playerNum - 1] = bot;
            bot.InitAI(Global.GameState.playerNum - 1, false);
        }

        [PunRPC]
        public bool RPCSetLocalPlayer(int index)
        {
            playerTank = Players[index];
            Global.GameState.playerID = index;
            playerCamera.Init(inputManager, Players[index].transform);
            statManager.Init(Players);
            crosshairManager.Init(playerTank.motion.tankComponents, playerCamera.GetComponentInChildren<Camera>());
            instructionManager.SetCamera(playerCamera);
            instructionSender.StartSending();
            return true;
        }
        [PunRPC]
        public void RPCSetPlayers(int[] SeatToID, int[] SyncerViewIDs)
        {
            for(int i = 0; i< SeatToID.Length; i++)
            {
                if(SeatToID[i]!=-1)
                {
                    int index = SeatToID[i];
                    Players[i] = 
                        Instantiate(playerPrefab, birthPoints.points[i].position,birthPoints.points[i].rotation).
                        GetComponentInChildren<Tank>();
                    Syncer syncer = PhotonView.Find(SyncerViewIDs[i]).transform.GetComponent<Syncer>();
                    Players[i].InitSyncMode(i, syncer);
                }
            }
        }
        [PunRPC]
        public void RPCSetBot(int botSeat,int botSyncerView)
        {
            Players[botSeat] = Instantiate(playerPrefab, birthPoints.points[botSeat].position, birthPoints.points[botSeat].rotation).
                        GetComponentInChildren<Tank>();
            Syncer syncer = PhotonView.Find(botSyncerView).transform.GetComponent<Syncer>();
            Players[botSeat].InitSyncMode(botSeat, syncer);
        }
        #endregion
    }
}
