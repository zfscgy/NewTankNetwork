using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.AI.StateMachine
{
    using ZF.MainGame.Base;
    using ZF.Communication;
    using UnityEngine.AI;

    public enum MotionStateIndices
    {
        going = 0,
        following = 1,
        stopping = 2,
        avoiding = 3,
        slowGoing = 4,
    }

    public enum ShootingStateIndices
    {
        shooting = 0,
        notShooting = 1,
    }

    public class TankStateInfo : StateInfo
    {
        public Instruction instruction;
        public Tank tank;
        public State lastState;
        public Transform destinationTransform;
        public Vector3 destinationPosition;
        public Transform targetTransform;
        public Tank targetTank;
        public float[] AvoidingTendencies;
    }

    public class TankState_Going : State
    {
        NavMeshPath navMeshPath;
        int currentNodeIndex;
        float avoidingLimit = 0.5f;
        float positionError = 10f;
        float angleError = 5f;
        float targetSpeed;
        bool isTrapped = false;
        public override void Start()
        {
            navMeshPath = new NavMeshPath();
            NavMesh.CalculatePath((stateInfo as TankStateInfo).tank.transform.position,
                (stateInfo as TankStateInfo).destinationPosition,
                NavMesh.AllAreas,
                navMeshPath);
            Debug.Log("Path Len: " + navMeshPath.corners.Length);
            currentNodeIndex = 1;
            if(navMeshPath.corners.Length < 2)
            {
                Vector3 normal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(
                    (stateInfo as TankStateInfo).tank.transform.position.x, (stateInfo as TankStateInfo).tank.transform.position.z);
                (stateInfo as TankStateInfo).AvoidingTendencies[0] = 10 * Mathf.Sign(normal.x);
                (stateInfo as TankStateInfo).AvoidingTendencies[1] = 10 * Mathf.Sign(normal.y);
            }
        }


        public override void Action()
        {
            if (currentNodeIndex > navMeshPath.corners.Length - 1)
            {
                return;
            }

            if (NextPosition() == 1)
            {
                Debug.Log("Current Node Index: " + currentNodeIndex);
                if (currentNodeIndex == navMeshPath.corners.Length - 1)
                {
                    return;
                }
                else
                {
                    CalculateTargetSpeed();
                    currentNodeIndex++;
                }
            }
        }
        public override State Decide()
        {
            if (Mathf.Abs((stateInfo as TankStateInfo).AvoidingTendencies[0]) > avoidingLimit
                || Mathf.Abs((stateInfo as TankStateInfo).AvoidingTendencies[1]) > avoidingLimit)
            {
                return (stateInfo as TankStateInfo).AvailableStates[(int)MotionStateIndices.avoiding];
            }
            if (navMeshPath.corners[navMeshPath.corners.Length - 1].AlmostEquals(
                (stateInfo as TankStateInfo).tank.transform.position, positionError))
            {
                return (stateInfo as TankStateInfo).AvailableStates[(int)MotionStateIndices.stopping];
            }
            return this;
        }

        /// <summary>
        /// Move towards the next node of the calculated navMeshPath
        /// Calculate the instruction.
        /// </summary>
        /// <returns> 1 if it already reaches the node, 0 for default</returns>
        private int NextPosition()
        {
            Vector3 currentPosition = (stateInfo as TankStateInfo).tank.transform.position;
            Vector3 currentRotation = (stateInfo as TankStateInfo).tank.transform.eulerAngles;
            float degree = Mathf.Atan2(navMeshPath.corners[currentNodeIndex].x - currentPosition.x,
                navMeshPath.corners[currentNodeIndex].z - currentPosition.z) * 180f / Mathf.PI - currentRotation.y;
            degree = degree % 360;
            if (degree < -180f)
            {
                degree = degree + 360f;
            }
            else if (degree > 180f)
            {
                degree = degree - 360f;
            }
            if ((navMeshPath.corners[currentNodeIndex] - currentPosition).magnitude < positionError)
            {
                //Debug.Log("Almost Equals!");
                (stateInfo as TankStateInfo).instruction.SetInstruction(0, 0, new Vector3());
                return 1;
            }
            else if (Mathf.Abs(degree) > angleError)
            {
                //Debug.Log("Steer and move!");
                if (degree < 0f)
                {
                    (stateInfo as TankStateInfo).instruction.SetKey(1 << 2);
                }
                else
                {
                    (stateInfo as TankStateInfo).instruction.SetKey(1 << 3);
                }
            }
            else
            {
                float distance = (navMeshPath.corners[currentNodeIndex] - currentPosition).magnitude;
                if (distance + targetSpeed < (stateInfo as TankStateInfo).tank.motion.GetSpeed() / 3)
                {
                    (stateInfo as TankStateInfo).instruction.SetKey(2);

                }
                else if (distance + targetSpeed < (stateInfo as TankStateInfo).tank.motion.GetSpeed())
                {
                    (stateInfo as TankStateInfo).instruction.SetKey(0);
                }
                else
                {
                    (stateInfo as TankStateInfo).instruction.SetKey(1);
                }
            }
            return 0;
        }

        /// <summary>
        /// Calculate the target speed of passing the next node
        /// </summary>
        private void CalculateTargetSpeed()
        {
            targetSpeed = 0f;
            if (currentNodeIndex + 1 < navMeshPath.corners.Length)
            {
                targetSpeed = 1f
                    + Vector3.Dot(navMeshPath.corners[currentNodeIndex] - (stateInfo as TankStateInfo).tank.transform.position, 
                    navMeshPath.corners[currentNodeIndex + 1] - navMeshPath.corners[currentNodeIndex]
                    ) /
                    ((navMeshPath.corners[currentNodeIndex] - (stateInfo as TankStateInfo).tank.transform.position).magnitude
                    * (navMeshPath.corners[currentNodeIndex + 1] - navMeshPath.corners[currentNodeIndex]).magnitude);
                targetSpeed *= 20f;
            }
            //Debug.Log("Target Speed:" + targetSpeed);
        }
    }

    public class TankState_Avoiding : State
    {
        float avoidingLimit = 0.5f;
        public override void Action()
        {
            int AD = 0;
            if ((stateInfo as TankStateInfo).AvoidingTendencies[1] > 0.5f)
            {
                AD = 2;
            }
            else if ((stateInfo as TankStateInfo).AvoidingTendencies[1] < -0.5f)
            {
                AD = 1;
            }
            if (Vector3.Dot((stateInfo as TankStateInfo).tank.motion.GetVelocity(),
                (stateInfo as TankStateInfo).tank.transform.forward) < 0)
            {
                AD = (AD % 2) + 1;
            }
            int WS = 0;
            if ((stateInfo as TankStateInfo).AvoidingTendencies[0] > 0.5f)  //This remains a question because when verticalTendency == 0 means you have let it stops
            {
                WS = 1;
            }
            else if ((stateInfo as TankStateInfo).AvoidingTendencies[0] < -0.5f)
            {
                WS = 2;
            }
            (stateInfo as TankStateInfo).instruction.SetKey((byte)(WS + (AD << 2)));
        }
        public override State Decide()
        {
            if (Mathf.Abs((stateInfo as TankStateInfo).AvoidingTendencies[0]) < avoidingLimit
                && Mathf.Abs((stateInfo as TankStateInfo).AvoidingTendencies[1]) < avoidingLimit)
            {
                return (stateInfo as TankStateInfo).lastState;
            }
            return this;
        }
    }

    public class TankState_Stoping : State
    {
        float avoidingLimit = 0.5f;
        public override void Start()
        {
            (stateInfo as TankStateInfo).instruction.SetKey(0);
        }
        public override void Action()
        {
            
        }
        public override State Decide()
        {
            if (Mathf.Abs((stateInfo as TankStateInfo).AvoidingTendencies[0]) > avoidingLimit
                || Mathf.Abs((stateInfo as TankStateInfo).AvoidingTendencies[1]) > avoidingLimit)
            {
                return (stateInfo as TankStateInfo).AvailableStates[(int)MotionStateIndices.avoiding];
            }
            return this;
        }
    }


    public class TankState_Shooting : State
    {
        float aimingInterval = 0.1f;
        float shootingInterval = 3.5f;
        float toleratingDistance = 10f;

        float lastShootingTime = -10f;
        float lastAimingTime = -10f;
        public override void Start()
        {
            lastAimingTime = Time.time - aimingInterval;
        }
        public override void Action()
        {
            if (Time.time - lastShootingTime > shootingInterval)
            {
                RaycastHit hit;
                Ray ray = new Ray((stateInfo as TankStateInfo).tank.motion.tankComponents.gun.position + 5 * (stateInfo as TankStateInfo).tank.motion.tankComponents.gun.forward,
                    (stateInfo as TankStateInfo).tank.motion.tankComponents.gun.forward);
                Physics.Raycast(ray, out hit, 400.0f, LayerMask.GetMask("Ground", "Tank"));
                if (hit.point != null)
                {
                    //Debug.DrawLine(tankGun.position, hit.point, Color.black);
                    //Debug.DrawLine(targetTransform.position, hit.point, Color.white);
                    if ((hit.point - (stateInfo as TankStateInfo).targetTank.transform.position).magnitude < toleratingDistance)
                    {
                        (stateInfo as TankStateInfo).instruction.SetMouse(1 << 2);
                        lastShootingTime = Time.time;
                        //Debug.Log("Shoot!");
                    }
                    else
                    {
                        (stateInfo as TankStateInfo).instruction.SetMouse(0);
                    }
                    //Debug.Log("Distance too far, Not shootable!" + (hit.point - targetTransform.position).magnitude);
                }

            }
            if (Time.time - lastAimingTime > aimingInterval)
            {
                (stateInfo as TankStateInfo).instruction.SetTargetPosition(
                    (stateInfo as TankStateInfo).targetTank.transform.position
                    + (stateInfo as TankStateInfo).targetTank.transform.up * 2.0f);
                lastAimingTime = Time.time;
            }
        }
        public override State Decide()
        {
            if ((stateInfo as TankStateInfo).targetTank.body.IsDead())
            {
                return (stateInfo as TankStateInfo).AvailableStates[(int)ShootingStateIndices.notShooting];
            }
            return this;
        }
    }

    public class TankState_NotShooting : State
    {
        float aimingInterval = 0.1f;

        float lastAimingTime = -10f;
        public override void Start()
        {
            lastAimingTime = Time.time - aimingInterval;
            (stateInfo as TankStateInfo).instruction.SetMouse(0);
        }
        public override void Action()
        {
            if (Time.time - lastAimingTime > aimingInterval)
            {
                (stateInfo as TankStateInfo).instruction.SetTargetPosition(
                    (stateInfo as TankStateInfo).tank.motion.tankComponents.gun.position
                    + 20 * (stateInfo as TankStateInfo).tank.transform.forward);
                lastAimingTime = Time.time;
            }
        }
        public override State Decide()
        {
            return this;
        }

    }

}
