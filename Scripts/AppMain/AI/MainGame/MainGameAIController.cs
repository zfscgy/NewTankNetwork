using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZF.AI
{

    using MainGame.Base;
    public class MainGameAIController : MonoBehaviour
    {
        public GameObject aiTankPrefab;
        public BirthPoints birthPoints;
        public Server.ServerMainController serverMainController;
        public MainGame.Client.MainGameLoader mainGameController;
        public void AddOneBot()
        {
            int seat = Global.GameState.playerNum++;
            Transform startTransform = birthPoints.points[seat];
            Tank newBot = Instantiate(aiTankPrefab, startTransform.position, startTransform.rotation).GetComponentInChildren<Tank>();
            if(Global.GameState.mode == Global.GameMode.isServer)
            {
                serverMainController.AddBot(newBot);
            }
            else
            {
                mainGameController.SetBotOffline(newBot);
            }
        }
        public void Init()
        {
            gameObject.SetActive(true);
        }
    }
}
