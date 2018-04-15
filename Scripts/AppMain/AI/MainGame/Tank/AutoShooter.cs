using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZF.AI
{
    using Communication;
    public class AutoShooter : MonoBehaviour
    {
        private Transform targetTransform;
        private Instruction instruction;
        public Transform tankGun;
        public float aimingInterval = 0.1f;
        public float shootingInterval = 3.5f;
        public float toleratingDistance = 10f;
        private bool isShootable = true;
        private float lastShootingTime = -10f;
        private float lastAimingTime = -10f;
        private void Update()
        {
            instruction.SetMouse(0);
            if (targetTransform != null && Time.time - lastAimingTime > aimingInterval)
            {
                instruction.SetTargetPosition(targetTransform.position + 2 * targetTransform.up);
                lastAimingTime = Time.time;
            }
            if (targetTransform == null)
            {
                instruction.SetTargetPosition(tankGun.position + 10 * transform.forward);
            }
            if (targetTransform != null)
            {
                if (TryIfShootable())
                {
                    isShootable = true;
                    if (Time.time - lastShootingTime > shootingInterval)
                    {
                        instruction.SetMouse(1 << 2);
                        lastShootingTime = Time.time;
                    }
                }
                else
                {
                    isShootable = false;
                }
            }
            else
            {
                isShootable = true;
            }
        }

        public void Init(Instruction _instruction)
        {
            instruction = _instruction;
        }

        public void SetTarget(Transform _targetTransform)
        {
            targetTransform = _targetTransform;
        }
        public void LoseTarget()
        {
            targetTransform = null;
        }

        private bool TryIfShootable()
        {
            RaycastHit hit;
            Ray ray = new Ray(tankGun.position + 5 * tankGun.forward, tankGun.forward);
            Physics.Raycast(ray, out hit, 400.0f, LayerMask.GetMask("Ground", "Tank"));
            if(hit.point!=null)
            {
                //Debug.DrawLine(tankGun.position, hit.point, Color.black);
                //Debug.DrawLine(targetTransform.position, hit.point, Color.white);
                if ((hit.point - targetTransform.position).magnitude < toleratingDistance)
                {
                    //Debug.Log("Shoot!");
                    return true;
                }
                //Debug.Log("Distance too far, Not shootable!" + (hit.point - targetTransform.position).magnitude);
            }

            return false;
        }

        public bool IsShootable()
        {
            return isShootable;
        }
    }
}
