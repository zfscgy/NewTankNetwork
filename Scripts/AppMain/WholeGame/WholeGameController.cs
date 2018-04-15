using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ZF.WholeGame
{
    using ZF.MainGame;
    using ZF.MainGame.Base;
    enum GameMode
    {
        OnLine,
        OffLine,
    }
    

    public class WholeGameController:MonoBehaviour
    {
        public GameObject GameStartController;
        public GameObject GameStartUI;
        public MainGameLoader mainGameLoader;
        private void Start()
        {
            DontDestroyOnLoad(GameStartController);
            DontDestroyOnLoad(GameStartUI);
            DontDestroyOnLoad(this.gameObject);
            Global.Singletons.wholeGameController = this;
            SceneManager.LoadScene("GameStart");
        }
        private void Update()
        {
            if(isBusy)
            {
                if(Time.time - startTime > waitingTime)
                {
                    waitingEvent();
                    isBusy = false;
                }
            }
        }

        public void LoadMainGame()
        {
            GameStartController.SetActive(false);
            GameStartUI.SetActive(false);
            if (Global.GameState.mode != Global.GameMode.inOfflineGame)
            {
                GameStartUI.transform.GetChild(2).GetComponentInChildren<StartGame.RoomManager>().enabled = false;
            }
        }

        public void LoadBackToStartScene()
        {
            GameStartUI.SetActive(true);
            GameStartController.SetActive(true);
            if (Global.GameState.mode != Global.GameMode.inOfflineGame)
            {
                GameStartUI.transform.GetChild(2).GetComponentInChildren<StartGame.RoomManager>().enabled = true;
            }
        }

        private bool isBusy = false;
        public delegate void WaitingEvent();
        private WaitingEvent waitingEvent;
        private float startTime;
        private float waitingTime;
        public bool WaitToExec(WaitingEvent _waitingEvent, float _waitingTime)
        {
            if(isBusy)
            {
                return false;
            }
            waitingEvent = _waitingEvent;
            waitingTime = _waitingTime;
            isBusy = true;
            return true;
        }

    }
}