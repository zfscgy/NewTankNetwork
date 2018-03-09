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
        private void Update()
        {
            if (Time.frameCount % 16 == 0)
            {
                if (targetTransform != null)
                {
                    instruction.SetTargetPosition(targetTransform.position);
                }
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
    }
}
