using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZF.Server
{
    using MainGame;
    using MainGame.Base;
    using Global;
    using Communication;
    using InstructionServer;
    public class ServerMainController : Photon.MonoBehaviour
    {
        int[] playerList;
        public BirthPoints birthPoints; 
        public GameObject playerPrefab;
        public GameObject syncerPrefab;
        public GameObject serverCameraPreafab;
        public InstructionReceiver instructionReceiver;
        public InputManager serverInputManager;
        Instruction[] clientInstructions = new Instruction[Global.playerPerRoom + 1];
        TankMotion[] clientTanks = new TankMotion[Global.playerPerRoom + 1];

        ServerCameraController serverCameraController;
        int[] SyncerViewIDs = new int[Global.playerPerRoom + 1];

        private void Start()
        {
            PhotonNetwork.sendRate = 100;
            PhotonNetwork.sendRateOnSerialize = 30;
            if (GameState.mode == GameMode.isServer)
            {
                Init();
            }
            else
            {
                enabled = false;
            }
        }

        public void Init()
        {
            playerList = GameState.PlayerIDs;
            /*At the beginning of the mainGame, the server will get playerList from server's roomController
             * Then instantiaite all the player tanks locally
             * */
            for(int i = 0; i < playerList.Length; i++)
            {
                int index = playerList[i];
                if (index != -1)
                {
                    Transform startTransform = birthPoints.points[i];
                    GameObject newPlayer = Instantiate(playerPrefab, startTransform.position,startTransform.rotation);
                    clientTanks[i] = newPlayer.GetComponentInChildren<TankMotion>();
                    clientTanks[i].SetInstruction(instructionReceiver.ReceivedInstructions[index]);
                    clientTanks[i].SetMode(TankMode.Server);
                    GameObject newSyncer = PhotonNetwork.Instantiate(syncerPrefab.name,
                        clientTanks[i].transform.position, clientTanks[i].transform.rotation, 0);
                    SyncerViewIDs[i] = newSyncer.GetPhotonView().viewID;
                    clientTanks[i].tankNetworkComponents.Set(newSyncer.transform.GetChild(0), newSyncer.transform.GetChild(1));
                }
            }
            serverCameraController = Instantiate(serverCameraPreafab, 
                birthPoints.points[0].position + new Vector3(0,5,0),
                birthPoints.points[0].rotation).GetComponent<ServerCameraController>();
            serverCameraController.Init(serverInputManager);
        }

        int ready = 0;
        [PunRPC]
        public void RPCOnePlayerReady()
        {
            ready++;
            if(ready == GameState.playerNum)
            {
                photonView.RPC("RPCSetPlayers", PhotonTargets.Others, playerList, SyncerViewIDs);
                for(int i = 0;i<playerList.Length;i++)
                {
                    if (playerList[i] != -1)
                    {
                        photonView.RPC("RPCSetLocalPlayer", PhotonPlayer.Find(playerList[i]), i);
                    }
                }
                instructionReceiver.StartListening();
            }
            
        }
    }
}