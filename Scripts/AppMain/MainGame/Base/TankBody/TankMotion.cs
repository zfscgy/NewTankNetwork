using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.MainGame.Base
{
    using ZF.Configs;
    using ZF.Communication;
    public enum TankMode
    {
        None,
        Local,
        Sync,
        Server,
    }
    public class TankMotion:MonoBehaviour
    {
        public TankConfig tankConfig;
        public TankComponents tankComponents;
        public TankNetworkComponents tankNetworkComponents;
        private Instruction instruction;
        private TankMode mode;
        public Rigidbody m_rigidbody;

        private Vector3[] lastPosition = new Vector3[2];
        private int posI = 0;
        private void Start()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            foreach(WheelRotate wheels in tankComponents.LeftWheels)
            {
                wheels.Init(tankConfig.speedMax, tankConfig.torqueMax);
            }
            foreach (WheelRotate wheels in tankComponents.RightWheels)
            {
                wheels.Init(tankConfig.speedMax, tankConfig.torqueMax);
            }
            lastPosition[posI] = transform.position;
            posI = (posI + 1) % 2;
        }

        private void FixedUpdate()
        {
            lastPosition[posI] = transform.position;
            posI = (posI + 1) % 2;
            if (mode == TankMode.Sync)
            {
                if (tankNetworkComponents.isSet)
                {
                    transform.position = tankNetworkComponents.referenceTankBody.localPosition;
                    transform.rotation = tankNetworkComponents.referenceTankBody.localRotation;
                    tankComponents.turret.localEulerAngles = 
                        new Vector3(0, tankNetworkComponents.referenceTurret.localEulerAngles.y, 0);
                    tankComponents.gun.localEulerAngles =
                         new Vector3(tankNetworkComponents.referenceTurret.localEulerAngles.x, 0, 0);
                }
            }
            else if (mode == TankMode.Server)
            {
                ExecuteInstruction();
                if (tankNetworkComponents.isSet)
                {
                    tankNetworkComponents.referenceTankBody.localPosition = transform.position;
                    tankNetworkComponents.referenceTankBody.localRotation = transform.rotation;
                    tankNetworkComponents.referenceTurret.localEulerAngles =
                        new Vector3(tankComponents.gun.localEulerAngles.x, tankComponents.turret.localEulerAngles.y, 0);
                }
            }
            else
            {
                ExecuteInstruction();
            }
        }
        #region Private Methods
        private void ExecuteInstruction()
        {
            if(instruction == null)
            {
                Debug.Log("No instruction specified!");
                return;
            }
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
            if(rateStep < 0)
            {
                steerDiff = -steerDiff;
            }
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
                    if (rateStep < tankConfig.autoParkingRate && m_rigidbody.velocity.magnitude < tankConfig.autoParkingSpeed)
                    {
                        lagCount += Time.fixedDeltaTime;
                        if (lagCount > tankConfig.autoParkingBrakeLag)
                        {
                            isAutoParking = true;
                            m_rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
                        }
                    }
                }
                else
                {
                    if(rateStep > tankConfig.autoParkingRate || m_rigidbody.velocity.magnitude > tankConfig.autoParkingSpeed)
                    {
                        isAutoParking = false;
                        m_rigidbody.constraints = RigidbodyConstraints.None;
                    }
                }
            }
            else
            {
                isAutoParking = false;
                m_rigidbody.constraints = RigidbodyConstraints.None;
            }
        }

        public Transform virtualTurret;
        public Transform virtualGun;
        private void ControlTurret(Vector3 targetPosition)
        {
            Quaternion origin = virtualTurret.localRotation;
            virtualTurret.LookAt(targetPosition);
            virtualTurret.localRotation = Quaternion.RotateTowards(origin, virtualTurret.localRotation, tankConfig.gunYSpeed * Time.fixedDeltaTime);
            virtualTurret.localEulerAngles = new Vector3(0, virtualTurret.localEulerAngles.y, 0);

            origin = virtualGun.localRotation;
            virtualGun.LookAt(targetPosition);
            virtualGun.localRotation = Quaternion.RotateTowards(origin, virtualGun.localRotation, tankConfig.gunXSpeed * Time.fixedDeltaTime);
            float angle_x = virtualGun.localEulerAngles.x > 180f ?
                virtualGun.localEulerAngles.x - 360f : virtualGun.localEulerAngles.x;
            angle_x = Mathf.Clamp(angle_x, -tankConfig.gunXMax, -tankConfig.gunXMin);
            tankComponents.turret.localEulerAngles = new Vector3(0, virtualTurret.localEulerAngles.y, 0);
            tankComponents.gun.localEulerAngles = new Vector3(angle_x, 0, 0);
        }

        #endregion
        public void SetMode(TankMode _mode)
        {

            mode = _mode;
            if(mode == TankMode.Sync)
            {
                GetComponent<Rigidbody>().isKinematic = true;
                foreach (WheelRotate wheels in tankComponents.LeftWheels)
                {
                    wheels.GetComponent<Rigidbody>().isKinematic = true;
                }
                foreach (WheelRotate wheels in tankComponents.RightWheels)
                {
                    wheels.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }
        public void SetInstruction(Instruction _instruction)
        {
            instruction = _instruction;
        }

        /////
        public byte GetSpeed()
        {
            return (byte)(int)((lastPosition[1] - lastPosition[0]).magnitude / Time.fixedDeltaTime * 3.6f);
        }
    }
}
