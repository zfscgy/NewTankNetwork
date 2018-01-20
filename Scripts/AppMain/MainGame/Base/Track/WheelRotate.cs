using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZF.MainGame.Base
{
    public class WheelRotate : MonoBehaviour
    {
        float maxAngVelocity;
        float maxTorque;
        Rigidbody thisRigidbody;
        Transform thisTransform;
        Transform parentTransform;
        Vector3 angles;

        void Awake()
        {
            thisRigidbody = GetComponent<Rigidbody>();
            thisTransform = transform;
            parentTransform = thisTransform.parent;
            angles = thisTransform.localEulerAngles;
        }


        public void Init(float _maxVelocity, float _maxTorque)
        {
            float radius = GetComponent<SphereCollider>().radius;
            maxAngVelocity = Mathf.Deg2Rad * ((_maxVelocity / (2.0f * Mathf.PI * radius)) * 360.0f);
            maxTorque = _maxTorque;
        }
        public void Rotate(float rate)
        {
            thisRigidbody.AddRelativeTorque(0.0f, rate * maxTorque, 0.0f);
            thisRigidbody.maxAngularVelocity = Mathf.Abs(maxAngVelocity * rate);
            // Stabilize angle.
            angles.y = thisTransform.localEulerAngles.y;
            thisRigidbody.rotation = parentTransform.rotation * Quaternion.Euler(angles);
        }

    }

}
