using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZF.Server
{
    using MainGame;
    using MainGame.Base;
    using MainGame.Base.Weapon;
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
        public MainGame.Stats.GameStatManager statManager;
        public ServerUIController serverUIController;
        public InputManager serverInputManager;
        Instruction[] clientInstructions = new Instruction[Global.playerPerRoom + 1];
        Tank[] clientTanks = new Tank[Global.playerPerRoom];

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
            instructionReceiver.Init();
            for(int i = 0; i < playerList.Length; i++)
            {
                int index = playerList[i];
                if (index != -1)
                {
                    Transform startTransform = birthPoints.points[i];
                    GameObject newPlayer = Instantiate(playerPrefab, startTransform.position,startTransform.rotation);
                    clientTanks[i] = newPlayer.GetComponentInChildren<Tank>();
                    Syncer newSyncer = PhotonNetwork.Instantiate(syncerPrefab.name,
                        clientTanks[i].transform.position, clientTanks[i].transform.rotation, 0).GetComponent<Syncer>();
                    SyncerViewIDs[i] = newSyncer.gameObject.GetPhotonView().viewID;
                    clientTanks[i].motion.tankNetworkComponents.Set(newSyncer);
                    clientTanks[i].InitOnServer(i, instructionReceiver.ReceivedInstructions[index], newSyncer);                    
                }
            }
            statManager.Init(clientTanks);
            serverCameraController = Instantiate(serverCameraPreafab, 
                birthPoints.points[0].position + new Vector3(0,5,0),
                birthPoints.points[0].rotation).GetComponent<ServerCameraController>();
            serverCameraController.Init(serverInputManager);
        }

        public void AddBot(Tank bot)
        {
            int seat = GameState.playerNum - 1;
            clientTanks[seat] = bot;
            Syncer newSyncer = PhotonNetwork.Instantiate(syncerPrefab.name,
                clientTanks[seat].transform.position, clientTanks[seat].transform.rotation, 0).GetComponent<Syncer>();
            int newSyncerID = newSyncer.photonView.viewID;
            bot.InitAI(seat, true, newSyncer);
            photonView.RPC("RPCSetBot", PhotonTargets.Others, seat, newSyncerID);
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
                        clientTanks[i].UpdateTankInfo();
                    }
                }
                instructionReceiver.StartListening();
                serverUIController.Init();
            }           
        }
    }
}