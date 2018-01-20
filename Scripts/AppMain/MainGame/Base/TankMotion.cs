using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.MainGame.Base
{
    using ZF.Configs;
    using ZF.Communication;
    enum TankMode
    {
        None,
        Local,
        Sync,
    }
    class TankMotion:MonoBehaviour
    {
        public TankConfig tankConfig;
        public TankComponents tankComponents;
        public TankNetworkComponents tankNetworkComponents;
        private Instruction instruction;
        private TankMode mode;
        private Rigidbody rigidbody;
        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            foreach(WheelRotate wheels in tankComponents.LeftWheels)
            {
                wheels.Init(tankConfig.speedMax, tankConfig.torqueMax);
            }
            foreach (WheelRotate wheels in tankComponents.RightWheels)
            {
                wheels.Init(tankConfig.speedMax, tankConfig.torqueMax);
            }
        }
        private void FixedUpdate()
        {
            if(mode == TankMode.Sync)
            {
                transform.position = tankNetworkComponents.referenceTankBody.position;
                tankComponents.turret.position = tankNetworkComponents.referenceTurret.position;
                return;
            }
            ExecuteInstruction();
        }
        #region Private Methods
        private void ExecuteInstruction()
        {
            Move(instruction.GetWS(),instruction.GetAD());
            ControlTurret(instruction.GetTargetPosition());
        }

        private float rateStep = 0f;
        private bool isAutoParking;
        private float lagCount = 0;
        private void Move(sbyte movement,sbyte steer)
        {
            //Debug.Log(movement.ToString() + "," + steer.ToString());
            if (rateStep*(float)movement < 0f || movement == 0)
            {
                rateStep = 0f;
            }
            else
            {
                rateStep += (float)movement * tankConfig.torqueIncreasingSpeed *Time.fixedDeltaTime;
                rateStep = Mathf.Clamp(rateStep, -1f, 1f);
            }
            float steerDiff = (float)steer * tankConfig.steerMax;
            foreach (WheelRotate wheels in tankComponents.LeftWheels)
            {
                wheels.Rotate(-(rateStep - steerDiff));
            }
            foreach (WheelRotate wheels in tankComponents.RightWheels)
            {
                wheels.Rotate(rateStep + steerDiff);
            }
            if (rateStep == 0f && steerDiff == 0f)
            {
                if (!isAutoParking)
                {
                    if (rateStep < tankConfig.autoParkingRate && rigidbody.velocity.magnitude < tankConfig.autoParkingSpeed)
                    {
                        lagCount += Time.fixedDeltaTime;
                        if (lagCount > tankConfig.autoParkingBrakeLag)
                        {
                            isAutoParking = true;
                            rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
                        }
                    }
                }
                else
                {
                    if(rateStep > tankConfig.autoParkingRate || rigidbody.velocity.magnitude > tankConfig.autoParkingSpeed)
                    {
                        isAutoParking = false;
                        rigidbody.constraints = RigidbodyConstraints.None;
                    }
                }
            }
            else
            {
                isAutoParking = false;
                rigidbody.constraints = RigidbodyConstraints.None;
            }
        }
        private void ControlTurret(Vector3 targetPosition)
        {

        }

        #endregion
        public void SetMode(TankMode _mode)
        {
            mode = _mode;
        }
        public void SetInstruction(Instruction _instruction)
        {
            instruction = _instruction;
        }
    }
}
