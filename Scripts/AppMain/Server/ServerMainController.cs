using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZF.Server
{
    using MainGame;
    using MainGame.Environment;
    using MainGame.Base;
    using MainGame.Base.Weapon;
    using Global;
    using Communication;
    using InstructionServer;
    public class ServerMainController : Photon.MonoBehaviour,IMainMaster
    {
        int[] playerList;
        public BirthPoints birthPoints; 
        public GameObject playerPrefab;
        public GameObject syncerPrefab;
        public GameObject serverCameraPreafab;
        public InstructionReceiver instructionReceiver;
        public MainGame.Stats.GameStatManager statManager;
        public ServerUIController serverUIController;
        public ServerInfoHUD serverInfoHUD;
        public InputManager serverInputManager;
        public SensoringSimulator sensoringSimulator;
        Instruction[] clientInstructions = new Instruction[Global.playerPerRoom + 1];
        Tank[] clientTanks = Singletons.gameRoutineController.GetTanks();
        ServerCameraController serverCameraController;
        int[] SyncerViewIDs = new int[Global.playerPerRoom + 1];

        //If server is offline, we do not assume that there will be any client player
        //There are just bots.
        //And the method about client players will never be invoked.
        //It is just a lazy way to create an all-bot game
        private bool isOffline = false;
        public void SetOfflineMode()
        {
            isOffline = true;
        }
        public void Init()
        {

            playerList = GameState.PlayerIDs;
            /*At the beginning of the mainGame, the server will get playerList from server's roomController
             * Then instantiaite all the player tanks locally
             * */
            if (!isOffline)
            {
                instructionReceiver.Init();
                for (int i = 0; i < playerList.Length; i++)
                {
                    int index = Singletons.gameRoutineController.RegisterNewTank(playerList[i]);
                    if (index != -1)
                    {
                        Transform startTransform = birthPoints.points[i];
                        GameObject newPlayer = Instantiate(playerPrefab, startTransform.position, startTransform.rotation);
                        clientTanks[i] = newPlayer.GetComponentInChildren<Tank>();
                        Syncer newSyncer = PhotonNetwork.Instantiate(syncerPrefab.name,
                            clientTanks[i].transform.position, clientTanks[i].transform.rotation, 0).GetComponent<Syncer>();
                        SyncerViewIDs[i] = newSyncer.gameObject.GetPhotonView().viewID;
                        clientTanks[i].motion.tankNetworkComponents.Set(newSyncer);
                        clientTanks[i].InitOnServer(i, instructionReceiver.ReceivedInstructions[index], newSyncer);
                    }
                }
            }
            else
            {
                serverUIController.Init();
                sensoringSimulator.Init(clientTanks);
            }
            statManager.Init(clientTanks);
            serverCameraController = Instantiate(serverCameraPreafab, 
                birthPoints.points[0].position + new Vector3(0,5,0),
                birthPoints.points[0].rotation).GetComponent<ServerCameraController>();
            serverCameraController.Init(serverInputManager);
            serverInfoHUD.Init(statManager.GetAllStats(), clientTanks, serverCameraController.GetComponent<Camera>());
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
                sensoringSimulator.Init(clientTanks);
            }           
        }

        bool IMainMaster.SetBot(Tank aiTank)
        {
            int botSeatID = Singletons.gameRoutineController.RegisterNewTank();
            if(botSeatID == -1)
            {
                return false;
            }
            aiTank.transform.position = birthPoints.points[botSeatID].position;
            aiTank.transform.rotation = birthPoints.points[botSeatID].rotation;
            clientTanks[botSeatID] = aiTank;
            if (!isOffline)
            {
                Syncer newSyncer = PhotonNetwork.Instantiate(syncerPrefab.name,
                    clientTanks[botSeatID].transform.position,
                    clientTanks[botSeatID].transform.rotation, 0).GetComponent<Syncer>();

                int newSyncerID = newSyncer.photonView.viewID;
                aiTank.InitAI(botSeatID, true, newSyncer);
                photonView.RPC("RPCSetBot", PhotonTargets.Others, botSeatID, newSyncerID);
            }
            else
            {
                aiTank.InitAI(botSeatID, false);
            }
            statManager.AddTank(aiTank);
            return true;
        }

        bool IMainMaster.DeleteBot(Tank aiTank)
        {
            throw new System.NotImplementedException();
        }
    }
}