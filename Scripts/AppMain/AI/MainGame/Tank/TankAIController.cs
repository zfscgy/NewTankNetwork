using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace ZF.AI
{
    using ZF.Communication;
    using ZF.MainGame.Base;
    using ZF.MainGame.Environment;
    public class TankAIController : MonoBehaviour
    {


        public Tank tank;
        public AutoNavigator navigator;
        public AutoShooter shooter;
        private Tank targetTank;
        private Transform destination;
        private Transform target;
        private SensoringSimulator sensoringSimulator;
        private Instruction instruction = new Instruction();

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            tank.motion.SetInstruction(instruction);
            navigator.Init(tank, instruction);
            shooter.Init(instruction);
            sensoringSimulator = GameObject.Find("MainGameAI").GetComponent<SensoringSimulator>();
        }

        private void Update()
        {
            if(Time.frameCount % 128 == 0)
            {
                if (target == null)
                {
                    FindTargetTank();
                    shooter.SetTarget(target);
                }
                navigator.SetNewDestination(destination.position);

            }
        }

        private void FindTargetTank()
        {
            int tankCount = 0;
            Tank[] TanksWithinRange = sensoringSimulator.FindTanksWithinDistance(transform.position, 100f, tank.seatID);
            while(TanksWithinRange[tankCount]!= null)
            {
                tankCount++;
            }
            if(tankCount > 0)
            {
                destination = TanksWithinRange[Random.Range(0, tankCount)].transform;
                target = destination;
            }
        }
        private void LoseTargetTank()
        {

        }
    }
}
