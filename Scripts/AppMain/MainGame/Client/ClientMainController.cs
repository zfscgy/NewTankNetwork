using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.MainGame.Client
{
    using Communication;
    using Base;
    public class ClientMainController:Photon.MonoBehaviour
    {
        public BirthPoints birthPoints;
        public GameObject playerPrefab;
        public GameObject cameraPrefab;
        public InstructionManager instructionManager;
        public InputManager inputManager;
        public InstructionSender instructionSender;
        public Stats.GameStatManager statManager;
        public UI.CrosshairManager crosshairManager;
        private Tank playerTank;
        private Tank[] Players = new Tank[Global.Global.playerPerRoom];
        private CameraController playerCamera;
        public void Init()
        {
            enabled = true;
            playerCamera = Instantiate(cameraPrefab).GetComponent<Base.CameraController>();
            instructionSender.Init(Global.GameSettings.serverIPAddress,instructionManager.GetInstruction(), PhotonNetwork.player.ID);
            photonView.RPC("RPCOnePlayerReady", PhotonTargets.MasterClient);
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
            for (int i = 0; i < SeatToID.Length; i++)
            {
                if (SeatToID[i] != -1)
                {
                    int index = SeatToID[i];
                    Players[i] =
                        Instantiate(playerPrefab, birthPoints.points[i].position, birthPoints.points[i].rotation).
                        GetComponentInChildren<Tank>();
                    Syncer syncer = PhotonView.Find(SyncerViewIDs[i]).transform.GetComponent<Syncer>();
                    Players[i].InitSyncMode(i, syncer);
                }
            }
        }
        [PunRPC]
        public void RPCSetBot(int botSeat, int botSyncerView)
        {
            Players[botSeat] = Instantiate(playerPrefab, birthPoints.points[botSeat].position, birthPoints.points[botSeat].rotation).
                        GetComponentInChildren<Tank>();
            Syncer syncer = PhotonView.Find(botSyncerView).transform.GetComponent<Syncer>();
            Players[botSeat].InitSyncMode(botSeat, syncer);
        }
    }
}
