using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ZF;
namespace ZF.MainGame.Client
{

    public class MainGameLoader:Photon.MonoBehaviour
    {
        public StartPoints[] StartPointsArray;
        public GameObject playerPrefab;
        public GameObject cameraPrefab;
        public Communication.InstructionManager instructionManager;
        public Communication.InputManager inputManager;
        public Communication.InstructionSender instructionSender;
        private Base.TankMotion playerTankMotion;
        private Base.TankMotion[] Players = new Base.TankMotion[Global.Global.playerPerRoom];
        private Base.CameraController playerCamera;
        private void Start()
        {
            if(Global.GameState.mode == Global.GameMode.isServer)
            {
                enabled = false;
                return;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GameObject player;
            playerCamera = Instantiate(cameraPrefab).GetComponent<Base.CameraController>();
            
            if (ZF.Global.GameState.mode == Global.GameMode.inOfflineGame)
            {
                player = Instantiate(playerPrefab, StartPointsArray[0].points[0].position, StartPointsArray[0].points[0].rotation);
                playerTankMotion = player.GetComponentInChildren<Base.TankMotion>(); 
                playerTankMotion.SetInstruction(instructionManager.GetInstruction());
                playerTankMotion.SetMode(Base.TankMode.Local);
                playerCamera.Init(inputManager,playerTankMotion.transform);
            }
            else
            {
                instructionSender.Init(Global.GameSettings.serverIPAddress, 
                    instructionManager.GetInstruction(), PhotonNetwork.player.ID);
                photonView.RPC("RPCOnePlayerReady", PhotonTargets.MasterClient);
            }

        }

        #region Public Methods
        [PunRPC]
        public bool RPCSetLocalPlayer(int index)
        {
            playerCamera.Init(inputManager, Players[index].transform);
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
                    int side = index / (Global.Global.playerPerRoom / 2);
                    int num = index / (Global.Global.playerPerRoom / 2);
                    Players[i] = 
                        Instantiate(playerPrefab, 
                        StartPointsArray[side].points[num].position, 
                        StartPointsArray[side].points[num].rotation).
                        GetComponentInChildren<Base.TankMotion>();
                    Players[i].SetMode(Base.TankMode.Sync);
                    Transform syncer = PhotonView.Find(SyncerViewIDs[i]).transform;
                    Players[i].tankNetworkComponents.Set(syncer.GetChild(0), syncer.GetChild(1));
                }
            }
        }

        #endregion
    }
}
