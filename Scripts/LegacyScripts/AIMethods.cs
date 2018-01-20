using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace ZF.AIMethods
{
    public static class AIMethods
    {
        public static Vector3 AutoTarget(TankControl controlledTank, Transform target)
        {
            Vector3 pos = target.position - controlledTank.transform.position;
            Vector3 relativePos = controlledTank.transform.InverseTransformVector(pos);
            return relativePos;
        }
        public static float CloselyMoveTo(TankControl controllerTank, Transform destination)
        {
            return 0f;
        }
    }
    public class PIDController
    {
        public float pPara;
        public float iPara;
        public float dPara;
        private float priorError;
        private float sumError;
        public PIDController(float _pPara,float _iPara, float _dPara)
        {
            pPara = _pPara;
            iPara = _iPara;
            dPara = _dPara;
        }
        public void Update(float error)
        {
            float val = pPara * error + iPara * sumError + dPara * (error - priorError);
            priorError = error;
            sumError += error;
        }
    }
}
