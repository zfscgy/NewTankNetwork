using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.AI
{
    using ZF.MainGame.Base;
    public class IntruderDetector:MonoBehaviour
    {
        public float radius_collision = 5f;
        public float factor_expectedCollisionTime = 5f;
        public float avoidanceExtensity = 3f;
        public float radius_extremelyNear = 3f;
        public float ratio_VerticalHorizental = 2f;
        public float additionalVerticalVelocity = 5f;
        public float factor_horizentalDuration = 0.6f;
        public float factor_verticalDuration = 0.95f;
        public bool isAI;
        public AutoNavigator navigator;
        public TankMotion motion;
        private List<TankMotion> IntruderTanks = new List<TankMotion>();
        private float[] Tendencies = new float[2] { 0, 0 };
        public float[] GetTendencies()
        {
            return Tendencies;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (isAI)
            {
                IntruderTanks.Add(other.GetComponent<IntruderDetector>().motion);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (isAI)
            {
                IntruderTanks.Remove(other.GetComponent<IntruderDetector>().motion);
            }
        }
        private void FixedUpdate()
        {
            transform.position = motion.transform.position;
            transform.rotation = motion.transform.rotation;
        }
        private void Update()
        {
            if (isAI)
            {
                Tendencies[0] *= factor_verticalDuration;
                Tendencies[1] *= factor_horizentalDuration;
                foreach (TankMotion intruderMotion in IntruderTanks)
                {
                    //navigator.MoveableDetected(intruderMotion.transform.position, intruderMotion.GetVelocity());
                    //Debug.Log("Moveable Detected!");
                    Vector3 intruderPosition = intruderMotion.transform.position;
                    Vector3 intruderVelocity = intruderMotion.GetVelocity();
                    //Debug.DrawLine(intruderPosition, intruderPosition + intruderVelocity, Color.gray);
                    Vector3 vec_dist = transform.position - intruderPosition;
                    Vector3 v = intruderVelocity - motion.GetVelocity();
                    float cosTheta = Vector3.Dot(v, vec_dist) / (v.magnitude * vec_dist.magnitude);
                    if (vec_dist.magnitude > radius_extremelyNear && cosTheta < 0.1f)
                    {
                        continue;
                    }
                    Vector3 estimatedCollisionPos;
                    if (vec_dist.magnitude <= radius_extremelyNear || v.magnitude < 0.5f)
                    {
                        estimatedCollisionPos = intruderPosition;
                    }
                    else
                    {
                        float tan2Theta = 1 / (cosTheta * cosTheta) - 1;
                        float delta = radius_collision * radius_collision * (1 + tan2Theta) - vec_dist.magnitude * vec_dist.magnitude * tan2Theta;
                        if (delta < 0f)
                        {
                            continue;
                        }
                        float x = (vec_dist.magnitude * tan2Theta + Mathf.Sqrt(delta)) / (1 + tan2Theta);
                        estimatedCollisionPos = intruderPosition + v / v.magnitude * (vec_dist.magnitude - x) / (1 + tan2Theta) * 1 / cosTheta;
                    }
                    //Debug.DrawLine(intruderPosition, estimatedCollisionPos);
                    //Debug.DrawLine(transform.position, estimatedCollisionPos, Color.red);
                    float estimateTime = (estimatedCollisionPos - intruderPosition).magnitude / (v.magnitude + additionalVerticalVelocity);
                    float v_towards = v.magnitude < 0.01f ? 0f : Vector3.Dot(v, transform.forward) / v.magnitude;
                    float v_off = v.magnitude < 0.01f ? 0f : Vector3.Dot(v, transform.right) / v.magnitude;
                    float collision_toward = Vector3.Dot((estimatedCollisionPos - transform.position), transform.forward);  // positive means in front , negative means behind
                    float collision_off = Vector3.Dot((estimatedCollisionPos - transform.position), transform.right); //positive means in right
                    if (Mathf.Abs(collision_off) < 0.3f)
                    {
                        collision_off = Mathf.Sign(collision_off) * 0.3f;
                    }
                    float myV_towards = Vector3.Dot(motion.GetVelocity(), transform.forward);
                    Tendencies[1] += - 0.3f * avoidanceExtensity * Mathf.Abs(v_off) * radius_collision / collision_off * factor_expectedCollisionTime / (1 + estimateTime);
                    if (vec_dist.magnitude <= radius_extremelyNear && Mathf.Abs(collision_off) / Mathf.Abs(collision_toward) < 0.5f)
                    {
                        //Debug.Log("Very Close and Vertical!");
                        Tendencies[0] += - avoidanceExtensity * Mathf.Sign(collision_toward)
                            * factor_expectedCollisionTime / (1 + estimateTime) * 20f * radius_collision / (radius_collision + 3f * vec_dist.magnitude);
                    }
                    else
                    {
                        Tendencies[0] += 0.3f *  avoidanceExtensity * myV_towards / additionalVerticalVelocity *
                            factor_expectedCollisionTime / (estimateTime + factor_expectedCollisionTime) + 0.5f * Mathf.Abs(Tendencies[1]);
                    }

                }
                Debug.DrawLine(transform.position + 3 * transform.up, transform.position + Tendencies[0] * transform.forward + 3 * transform.up);
                Debug.DrawLine(transform.position + 3 * transform.up, transform.position + Tendencies[1] * transform.right + 3 * transform.up);
            }
        }
    }
}
