using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace ZF.AI
{
    using ZF.Communication;
    using ZF.MainGame.Base;
    [System.Serializable]
    enum Mode
    {
        direct,
        stop,
        slow,
        avoiding,
    }
    /// <summary>
    /// Navigator to help tank go to the destination and to avoid collision with other tanks.
    /// Its performance is very poor!
    /// </summary>
    public class AutoNavigator:MonoBehaviour
    {
        public Vector3 targetRotation;
        public Tank tank;
        public Instruction instruction;

        public float positionError = 10f;
        public float angleError = 5f;

        public float collisionR = 15f;
        public float avoidanceT = 5f;
        public float avoidanceR = 3f;
        public float avoidanceRNear = 3f;
        public float avoidVHRatio = 2f;
        public float additionalV = 5f;
        public float horizentalDuration = 0.9f;
        public float verticalDuration = 0.95f;

        
        private Mode mode = Mode.stop;
        private Vector3 destination;
        private NavMeshPath navMeshPath;
        private Vector3 targetPosition;
        private float targetSpeed;
        private int next = 1;
        private float horizontalTendency = 0f;
        private float verticalTendency = 0f;
        private Mode lastMode;
        private void Update()
        {
            //Debug.Log("Tendencies" + verticalTendency + " " + horizontalTendency);
            if (mode != Mode.avoiding && (Mathf.Abs(horizontalTendency) > 0.5f || Mathf.Abs(verticalTendency) > 0.5f))
            {
                lastMode = mode;
                mode = Mode.avoiding;
            }
            if (mode == Mode.avoiding && (Mathf.Abs(horizontalTendency) <= 0.5f && Mathf.Abs(verticalTendency) <= 0.5f))
            {
                mode = lastMode;
                if(mode == Mode.direct)
                {
                    SetNewDestination(destination);
                }
            }
            if (mode == Mode.stop)
            {
                instruction.SetKey(0);
                return;
            }
            else if (mode == Mode.direct)
            {
                Debug.DrawLine(tank.transform.position, navMeshPath.corners[next - 1], Color.blue);
                for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
                {
                    Debug.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i + 1], Color.red);
                }
                if (NextPosition() == 1)
                {
                    if (next == navMeshPath.corners.Length)
                    {
                        targetPosition = destination;
                        CalculateTargetSpeed();
                        mode = Mode.stop;
                        return;
                    }
                    else
                    {
                        CalculateTargetSpeed();
                        targetPosition = navMeshPath.corners[next++];
                    }
                }
            }
            else if(mode == Mode.avoiding)
            {
                int AD = 0;
                if(horizontalTendency > 0.5f)
                {
                    AD = 1;
                }
                else if(horizontalTendency < -0.5f)
                {
                    AD = 2;
                }
                if(Vector3.Dot(tank.motion.GetVelocity() , tank.transform.forward) < 0)
                {
                    AD = (AD % 2) + 1;
                }
                int WS = 0;
                if(verticalTendency > 0.5f)  //This remains a question because when verticalTendency == 0 means you have let it stops
                {
                    WS = 2;
                }
                else if(verticalTendency < -0.5f)
                {
                    WS = 1;
                }
                instruction.SetKey((byte)(WS + (AD << 2)));
                horizontalTendency *= horizentalDuration;
                verticalTendency *= verticalDuration;
            }
        }

        public void Init(Tank _tank, Instruction _instruction)
        {
            tank = _tank;
            instruction = _instruction;
            navMeshPath = new NavMeshPath();
        }

        public void SetNewDestination(Vector3 _destination)
        {
            next = 1;
            destination = _destination;
            //Debug.Log("Path length:" + navMeshPath.corners.Length);
            NavMesh.CalculatePath(tank.transform.position, destination, NavMesh.AllAreas, navMeshPath);
            CalculateTargetSpeed();
            targetPosition = navMeshPath.corners[next++];
            mode = Mode.direct;
        }
        public void SetStopMode()
        {
            mode = Mode.stop;
        }

        private int NextPosition()
        {
            Vector3 currentPosition = tank.transform.position;
            Vector3 currentRotation = tank.transform.eulerAngles;
            float degree = Mathf.Atan2(targetPosition.x - currentPosition.x,targetPosition.z - currentPosition.z ) * 180f/Mathf.PI - currentRotation.y;
            degree = degree % 360;
            if (degree < -180f)
            {
                degree = degree + 360f;
            }
            else if (degree > 180f)
            {
                degree = degree - 360f;
            }
            //Debug.Log("Distance:" + (targetPosition - currentPosition).magnitude);
            if ((targetPosition - currentPosition).magnitude < positionError)
            {
                //Debug.Log("Almost Equals!");
                instruction.SetInstruction(0, 0, new Vector3());
                return 1;
            }
            else if (Mathf.Abs(degree) > angleError)
            {
                //Debug.Log("Steer and move!");
                if(degree < 0f)
                {
                    instruction.SetKey(1 << 2);
                }
                else
                {
                    instruction.SetKey(1 << 3);
                }
            }
            else
            {
                float distance = (targetPosition - currentPosition).magnitude;
                if(distance + targetSpeed < tank.motion.GetSpeed() / 3)
                {
                    instruction.SetKey(2);

                }
                else if (distance + targetSpeed < tank.motion.GetSpeed())
                {
                    instruction.SetKey(0);
                }
                else
                {
                    instruction.SetKey(1);
                }
            }
            return 0;
        }

        private void CalculateTargetSpeed()
        {
            targetSpeed = 0f;
            if (next + 1 < navMeshPath.corners.Length)
            {
                targetSpeed = 1f + Vector3.Dot(navMeshPath.corners[next] - tank.transform.position, navMeshPath.corners[next + 1] - navMeshPath.corners[next]) /
                    ((navMeshPath.corners[next] - tank.transform.position).magnitude * (navMeshPath.corners[next + 1] - navMeshPath.corners[next]).magnitude);
                targetSpeed *= 20f;
            }
            //Debug.Log("Target Speed:" + targetSpeed);
        }

        public void MoveableDetected(Vector3 position, Vector3 velocity)
        {
            //Debug.Log("Moveable Detected!");
            Debug.DrawLine(position, position + velocity, Color.gray);
            Vector3 dist = tank.transform.position - position;
            Vector3 v = velocity - tank.motion.GetVelocity();
            float cosTheta = Vector3.Dot(v, dist) / (v.magnitude * dist.magnitude);
            if (cosTheta < 0.1f)
            {
                return;
            }
            Vector3 estimatedPos;
            if (dist.magnitude <= 3 * collisionR + 0.1f || v.magnitude < 0.1f)
            {
                estimatedPos = position;
            }
            else
            {
                float tan2Theta = 1 / (cosTheta * cosTheta) - 1;
                float delta = collisionR * collisionR * (1 + tan2Theta) - dist.magnitude * dist.magnitude * tan2Theta;
                if (delta < 0f)
                {
                    return;
                }
                float x = (dist.magnitude * tan2Theta + Mathf.Sqrt(delta)) / (1 + tan2Theta);
                estimatedPos = position + v / v.magnitude * (dist.magnitude - x) / (1 + tan2Theta) * 1 / cosTheta;
            }
            //Debug.DrawLine(position, estimatedPos);
            //Debug.DrawLine(tank.transform.position, estimatedPos, Color.red);
            float estimateTime = (estimatedPos - position).magnitude / (v.magnitude + additionalV);
            float v_towards = v.magnitude < 0.01f ? 0f : Vector3.Dot(v, tank.transform.forward) / v.magnitude;
            float v_off = v.magnitude < 0.01f ? 0f : Vector3.Dot(v, tank.transform.right) / v.magnitude;
            float collision_toward = Vector3.Dot((estimatedPos - tank.transform.position), tank.transform.forward);  // positive means in front , negative means behind
            float collision_off = Vector3.Dot((estimatedPos - tank.transform.position), tank.transform.right); //positive means in right
            if(Mathf.Abs(collision_off) < 0.3f)
            {
                collision_off -= Mathf.Sign(collision_off) * 0.3f;
            }
            float myV_towards = Vector3.Dot(tank.motion.GetVelocity(), tank.transform.forward);
            horizontalTendency += avoidanceR * Mathf.Abs(v_towards) * collisionR / collision_off;
            if (dist.magnitude < 3 * collisionR && Mathf.Abs(collision_off)/Mathf.Abs(collision_toward) < 0.5f)
            {
                //Debug.Log("Very Close and Vertical!");
                verticalTendency += avoidanceR * avoidanceRNear * Mathf.Sign(collision_toward);
            }
            else
            {
                verticalTendency -= avoidanceR * myV_towards / additionalV * avoidanceT / (estimateTime + avoidanceT) + 0.5f * Mathf.Abs(horizontalTendency);
            }
        }

    }
}
