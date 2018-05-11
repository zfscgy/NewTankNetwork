using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.MainGame
{
    using Client;
    using Server;
    using Offline;
    using AI;
    using Global;
    using WholeGame.GameReplay;
    public class MainGameLoader:Photon.MonoBehaviour
    {
        public ClientMainController clientMainController;
        public ServerMainController serverMainController;
        public OfflineMainController offlineMainController;
        public MainGameAIController mainGameAIController;
        public GameReplayer gameReplayer;
        private void Awake()
        {
            Singletons.wholeGameController.mainGameLoader = this;
            Singletons.wholeGameController.LoadMainGame();
        }
        private void Start()
        {
            PhotonNetwork.sendRate = 100;
            PhotonNetwork.sendRateOnSerialize = 30;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if(GameState.mode == GameMode.inRoom)
            {
                clientMainController.Init();
            }
            else if(GameState.mode == GameMode.inOfflineGame)
            {
                offlineMainController.Init();
                mainGameAIController.Init(offlineMainController as IMainMaster);
            }
            else if(GameState.mode == GameMode.asOfflineAIServer)
            {
                serverMainController.SetOfflineMode();
                serverMainController.Init();
                mainGameAIController.Init(serverMainController as IMainMaster);
            }
            else if(GameState.mode == GameMode.isServer)
            {
                serverMainController.Init();
                mainGameAIController.Init(serverMainController as IMainMaster);
            }
            else if(GameState.mode == GameMode.isPlayBack)
            {
                gameReplayer.Init(GameState.saveFilename);
            }
        }

        public void Restart()
        {
            Base.Tank[] AllTanks = Singletons.gameRoutineController.GetTanks();
            for(int i = 0; i< AllTanks.Length; i++)
            {
                if (AllTanks[i] != null)
                {
                    Destroy(AllTanks[i].transform.parent.gameObject);
                    AllTanks[i] = null;
                }
            }
            Destroy(GameObject.Find("ServerCamera"));
            Start();
        }
    }
}
