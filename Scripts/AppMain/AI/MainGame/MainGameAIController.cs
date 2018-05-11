using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZF.AI
{
    using ZF.MainGame;
    using ZF.MainGame.Base;
    public class MainGameAIController : MonoBehaviour
    {
        public GameObject aiTankPrefab;
        public BirthPoints birthPoints;
        public Transform AIInitialDestination;
        private IMainMaster mainMaster;
        private List<TankAIController> TankAIControllers = new List<TankAIController>();
        public void Init(IMainMaster _mainMaster)
        {
            TankAIControllers.Clear();
            mainMaster = _mainMaster;
            gameObject.SetActive(true);
        }
        public void AddOneBot()
        {
            Tank newBot = Instantiate(aiTankPrefab).GetComponentInChildren<Tank>();
            if(mainMaster.SetBot(newBot))
            {
                TankAIControllers.Add(newBot.AIController);
            }
            else
            {
                Destroy(newBot);
            }
        }
        public bool AddOneBot(int id)
        {
            Tank newBot = Instantiate(aiTankPrefab).GetComponentInChildren<Tank>();
            if (mainMaster.SetBot(newBot, id))
            {
                TankAIControllers.Add(newBot.AIController);
                return true;
            }
            else
            {
                Destroy(newBot);
                return false;
            }

        }
        public void StartAllAI()
        {
            for (int i = 0; i < TankAIControllers.Count; i++)
            {
                TankAIControllers[i].DestinationPosition = AIInitialDestination.position;
                TankAIControllers[i].StartAI();
            }
        }
        public void StopAllAIs()
        {
            for (int i = 0; i < TankAIControllers.Count; i++)
            {
                TankAIControllers[i].StopAI();
            }
        }
    }
}
