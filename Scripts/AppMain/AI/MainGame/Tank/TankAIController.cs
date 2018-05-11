using System.Collections.Generic;
using UnityEngine;
namespace ZF.AI
{
    using Configs;
    using StateMachine;
    using ZF.Communication;
    using ZF.MainGame.Base;
    using ZF.MainGame.Environment;
    public enum TankAIState
    {
        doing,
        waiting,
    }
    public class TankAIController : MonoBehaviour, IStateController
    {
        public AIControllerConfigs configs;


        public Tank tank;
        public IntruderDetector intruderDetector;

        private SensoringSimulator sensoringSimulator;
        private Instruction instruction = new Instruction();

        private TankAIState state = TankAIState.waiting;
        private TankStateInfo tankStateInfo_motion;
        private TankStateInfo tankStateInfo_shooting;
        private List<Tank> ObservedEnemyTanks = new List<Tank>();

        private Transform destinationTransform;
        public Transform DestinationTransform
        {
            get
            {
                return destinationTransform;
            }
            set
            {
                destinationTransform = value;
                if(tankStateInfo_motion!= null)
                {
                    tankStateInfo_motion.destinationTransform = destinationTransform;
                }
            }
        }
        private Vector3 destinationPosition;
        public Vector3 DestinationPosition
        {
            get { return destinationPosition; }
            set
            {
                destinationPosition = value;
                if (tankStateInfo_motion != null)
                {
                    tankStateInfo_motion.destinationPosition = destinationPosition;
                }
            }
        }
        private Transform TargetTransform;
        private Tank targetTank;
        public Tank TargetTank
        {
            get { return targetTank; }
            set
            {
                targetTank = value;
                if (tankStateInfo_shooting != null)
                {
                    tankStateInfo_shooting.targetTank = targetTank;
                }
            }
        }

        public State[] MotionStates = new State[5]
        {
            new TankState_Going(), null, new TankState_Stoping(), new TankState_Avoiding(), null,
        };
        public State[] ShootingStates = new State[2]
        {
            new TankState_Shooting(), new TankState_NotShooting(),
        };
        private State currentMotionState;
        private State currentShootingState;
        public void Init(TankAIState _state)
        {
            state = _state;
            tank.motion.SetInstruction(instruction);
            tank.weapon.Init(instruction);
            sensoringSimulator = GameObject.Find("MainGameAI").GetComponent<SensoringSimulator>();


            currentMotionState = MotionStates[(int)MotionStateIndices.going];
            currentShootingState = ShootingStates[(int)ShootingStateIndices.notShooting];

            tankStateInfo_motion = new TankStateInfo
            {
                instruction = this.instruction,
                tank = this.tank,
                AvailableStates = this.MotionStates,
                AvoidingTendencies = this.intruderDetector.GetTendencies(),
                destinationPosition = this.DestinationPosition,
                destinationTransform = this.destinationTransform
            };

            for (int i = 0; i < MotionStates.Length; i++)
            {
                if (MotionStates[i] != null)
                {
                    MotionStates[i].Init(tankStateInfo_motion);
                }
            }

            tankStateInfo_shooting = new TankStateInfo
            {
                instruction = this.instruction,
                tank = this.tank,
                AvailableStates = this.ShootingStates,
                targetTank = this.TargetTank,
                targetTransform = this.TargetTransform
            };

            for (int i = 0; i < ShootingStates.Length; i++)
            {
                if (ShootingStates[i] != null)
                {
                    ShootingStates[i].Init(tankStateInfo_shooting);
                }
            }
        }

        private float lastCheckTime = -10f;
        private float toleratingDistance = 10;
        private float keepingDistance = 200f;
        private void Update()
        {
            if (state == TankAIState.waiting)
            {
                return;
            }

            if (Time.time - lastCheckTime > configs.checkInterval)
            {
                sensoringSimulator.FindEnemyTanksWithinDistance(
                    transform.position, configs.findingDistance, ObservedEnemyTanks, tank.seatID);
                sensoringSimulator.LoseEnemyTanksWithoutDistance(
                    transform.position, configs.findingDistance, ObservedEnemyTanks);
                if (ObservedEnemyTanks.Contains(TargetTank) && TryIfShootable(TargetTank.transform.position) && ( targetTank.transform.position - transform.position).magnitude > keepingDistance)
                {
                    if(currentMotionState == MotionStates[(int)MotionStateIndices.going])
                    {
                        currentMotionState = MotionStates[(int)MotionStateIndices.stopping];
                        currentMotionState.Start();
                    }
                }
                else if(ObservedEnemyTanks.Contains(TargetTank) && 
                    (!TryIfShootable(TargetTank.transform.position)|| (targetTank.transform.position - transform.position).magnitude > keepingDistance))
                {
                    DestinationPosition = TargetTank.transform.position;
                    currentMotionState = MotionStates[(int)MotionStateIndices.going];
                    currentMotionState.Start();
                }
                else
                {
                    for (int i = 0; i < ObservedEnemyTanks.Count; i++)
                    {
                        if (TryIfShootable(ObservedEnemyTanks[i].transform.position))
                        {
                            TargetTank = ObservedEnemyTanks[i];
                            currentShootingState = ShootingStates[(int)ShootingStateIndices.shooting];
                            currentShootingState.Start();
                            break;
                        }
                    }
                }
                lastCheckTime = Time.time;
            }

            currentMotionState.Action();
            State nextState = currentMotionState.Decide();
            if (nextState != currentMotionState)
            {
                tankStateInfo_motion.lastState = currentMotionState;
                currentMotionState = nextState;
                currentMotionState.Start();
            }
            currentShootingState.Action();
            nextState = currentShootingState.Decide();
            if (nextState != currentShootingState)
            {
                currentShootingState = nextState;
                currentShootingState.Start();
            }
        }

        public void StartAI()
        {
            state = TankAIState.doing;
            currentMotionState.Start();
            currentShootingState.Start();
        }

        public void StopAI()
        {
            state = TankAIState.waiting;
        }

        void IStateController.Order()
        {
            throw new System.NotImplementedException();
        }

        StateInfo IStateController.GetInfo()
        {
            throw new System.NotImplementedException();
        }


        private bool TryIfShootable(Vector3 _targetPosition)
        {
            RaycastHit hit;
            Ray ray = new Ray(tank.motion.tankComponents.gun.position + 5 * tank.motion.tankComponents.gun.forward,
                _targetPosition - tank.motion.tankComponents.gun.position);
            Physics.Raycast(ray, out hit, 400.0f, LayerMask.GetMask("Ground", "Tank"));
            if (hit.point != null)
            {
                //Debug.DrawLine(tankGun.position, hit.point, Color.black);
                //Debug.DrawLine(targetTransform.position, hit.point, Color.white);
                if ((hit.point - _targetPosition).magnitude < toleratingDistance)
                {
                    //Debug.Log("Shoot!");
                    return true;
                }
                //Debug.Log("Distance too far, Not shootable!" + (hit.point - targetTransform.position).magnitude);
            }
            return false;
        }
    }
}
