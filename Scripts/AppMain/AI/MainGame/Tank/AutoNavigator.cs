using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace ZF.AI
{
    using ZF.Communication;
    using ZF.MainGame.Base;
    [System.Serializable]
    public class AutoNavigator:MonoBehaviour
    {
        public Vector3 targetRotation;
        public Tank tank;
        public Instruction instruction;

        public float positionError = 10f;
        public float angleError = 5f;

        private bool isNavigating = false;
        private Vector3 destination;
        private NavMeshPath navMeshPath;
        private Vector3 targetPosition;
        private float targetSpeed;
        private int next = 1;

        private void Update()
        {
            if(!isNavigating)
            {
                return;
            }
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
                    isNavigating = false;
                    return;
                }
                else
                {
                    CalculateTargetSpeed();
                    targetPosition = navMeshPath.corners[next++];                   
                }
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
            NavMesh.CalculatePath(tank.transform.position, destination, NavMesh.AllAreas, navMeshPath);
            Debug.Log("Path length:" + navMeshPath.corners.Length);
            CalculateTargetSpeed();
            targetPosition = navMeshPath.corners[next++];
            isNavigating = true;
        }

        public int NextPosition()
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
            Debug.Log("Distance:" + (targetPosition - currentPosition).magnitude);
            if ((targetPosition - currentPosition).magnitude < positionError)
            {
                Debug.Log("Almost Equals!");
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
    }
}
