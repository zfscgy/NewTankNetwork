using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace ZF.Trash.AI
{
    using ZF.Communication;
    using ZF.MainGame.Base;
    using ZF.MainGame.Environment;
    public enum TankAIState
    {
        chasing,
        going,
        defending,
        waiting,
        stoping,
    }
    /*
    public class TankAIController : MonoBehaviour
    {
        public Tank tank;
        public AutoNavigator navigator;
        public AutoShooter shooter;
        public float checkingInterval = 3f;
        public float findingInterval = 20f;
        private Tank targetTank;
        private Transform destination;
        private Transform target;
        private SensoringSimulator sensoringSimulator;
        private Instruction instruction = new Instruction();
        private TankAIState state;

        private float lastCheckedTime = -10f;
        private float lastFindingTime = -10f;
        private void Start()
        {
            //Init(TankAIState.going);
        }

        public void Init(TankAIState _state)
        {
            state = _state;
            tank.motion.SetInstruction(instruction);
            tank.weapon.Init(instruction);
            navigator.Init(tank, instruction);
            shooter.Init(instruction);
            sensoringSimulator = GameObject.Find("MainGameAI").GetComponent<SensoringSimulator>();
            if(state == TankAIState.going)
            {
                destination = sensoringSimulator.warPoint;
                navigator.SetNewDestination(destination.position);
            }
        }

        private void Update()
        {
            if(Time.time - lastCheckedTime > checkingInterval)
            {
                lastCheckedTime = Time.time;
                if (target == null || targetTank.body.IsDead())
                {
                    FindTargetTank();
                    shooter.SetTarget(target);
                    if (state == TankAIState.chasing)
                    {
                        destination = target;
                        navigator.SetNewDestination(destination.position);
                    }
                    else if(state == TankAIState.stoping)
                    {
                        navigator.SetStopMode();
                    }
                    else
                    {
                        navigator.SetNewDestination(destination.position);
                    }
                }
                if(!shooter.IsShootable())
                {
                    state = TankAIState.chasing;
                }
                else if(state == TankAIState.chasing)
                {
                    state = TankAIState.stoping;
                }
            }
            if(Time.time - lastFindingTime > findingInterval)
            {
                lastFindingTime = Time.time;
                FindTargetTank();
                shooter.SetTarget(target);
                if (state == TankAIState.chasing)
                {
                    destination = target;
                    navigator.SetNewDestination(destination.position);
                }
            }
        }

        private void FindTargetTank()
        {
            int tankCount = 0;
            Tank[] TanksWithinRange = sensoringSimulator.FindEnemyTanksWithinDistance(transform.position, 500f, tank.seatID);
            while(tankCount < TanksWithinRange.Length && TanksWithinRange[tankCount]!= null)
            {
                tankCount++;
            }
            if(tankCount > 0)
            {
                targetTank = TanksWithinRange[Random.Range(0, tankCount)];
                target = targetTank.transform;
            }
            else
            {
                targetTank = null;
                target = null;
            }
        }
        private void LoseTargetTank()
        {

        }
        public void Stop()
        {
            state = TankAIState.stoping;
            navigator.enabled = false;
            shooter.enabled = false;
            enabled = false;
        }
    }*/
}
