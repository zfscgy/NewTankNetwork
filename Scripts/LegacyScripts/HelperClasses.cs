using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
namespace ZF
{
#region For TankControl
    [System.Serializable]
    public class TankParameters
    {
        //Shooting
        public float shootInterval;
        //Gun
        public float gunYSpeed;
        public float gunXSpeed;
        public float gunXSppedMax;
        public float gunXSpeedMin;
        //Motion
        public float maxTorque;
        public float maxSteer;
        public float maxSpeed;
    }
    [System.Serializable]
    public class PlayerUI
    {
        //
        public Text countDownText;
        public Texture2D crosshead;
        public Texture2D shootingBar;
        //Display info
        public Text[] WeaponTexts = new Text[3];
        public Text OutputText;
        public Text healthText;
        public Text turretHealthText;
        public Text bottomHealthText;
    }
    [System.Serializable]
    public class TankComponents
    {
        TankHealth health;
        TankWeapons weapons;
    }
    [System.Serializable]
    public class Wheel
    {
        public bool motor;
        public bool steering;
        public WheelCollider LeftWheel;
        public WheelCollider RightWheel;
    }

    public struct TankOrder
    {
        public int move;
        public int steer;
        public Vector3 direction;
        public int shoot;
    }

    public struct TankState
    {
        public Vector3 tankPos;
        public Vector3 tankAngle;
        public Vector3 TurretAngle;
        public Vector3 tankSpeed;
    }
#endregion
    //This class is for the card display in the waiting scene which contains player informations.
    [System.Serializable]
    public class PlayerCard
    {
        public GameObject card;
        public int id;
        public int used = 0;
    }

    public enum Flag
    {
        Red = 0,
        Blue = 1
    }

    public enum TankBody
    {
        Bottom = 0,
        Turret = 1,
        IsNotTankBody = 5
    }

    [System.Serializable]
    public class TankBodyPart
    {
        public string name;
        public int health = 100;
        public float protection = 1f;
        public float ratioToTotalHealth = 0.5f;
        //Returns the damage to the totalHealth
        public int TakeDamage(int damage, out int partDamage)
        {
            partDamage = (int)(damage / protection);
            health -= (health > partDamage)?partDamage:health;
            return (int)(partDamage * ratioToTotalHealth);
        }
        public delegate void DeathEffectToTank();
        public TankBodyPart(string _name)
        {
            name = _name;
        }
    }

    [System.Serializable]
    public class Weapon
    {
        public string name;
        //In order to use PhotonNetwork, bulletPrefab must be in the /Resource folder
        public GameObject bulletPrefab;
        public int damage;
        public int number;
        public bool GetOne()
        {
            if (number <= 0)
            {
                return false;
            }
            else
            {
                number--;
                return true;
            }
        }
    }


    //After hit a tank we will use a RPC to call back, and the parameter is this struct
    [System.Serializable]
    public struct HitInfo
    {
        public ushort shooterDamage;
        public ushort shooterViewID;
        public byte shooterNetworkID;
        public byte victimNetworkID;
        public TankBody hitPart;
        public ushort partDamage;
        public ushort totalDamage;
        public byte victimState;
        public byte[] ToBytes()
        {
            return new byte[12] { (byte)(shooterDamage>>8),(byte)(shooterDamage & 0xff),
                (byte)(shooterViewID>>8),(byte)(shooterViewID & 0xff), shooterNetworkID,
                victimNetworkID,(byte)hitPart,
                (byte)(partDamage>>8),(byte)(partDamage & 0xff),
                (byte)(totalDamage>>8),(byte)(totalDamage & 0xff),
                victimState };
        }
        public static HitInfo BytesToHitInfo(byte[] Bytes)
        {
            if(Bytes.Length != 12)
            {
                return new HitInfo();
            }
            HitInfo hitInfo = new HitInfo();
            hitInfo.shooterDamage = (ushort)(((uint)Bytes[0] << 8) + (uint)Bytes[1]);
            hitInfo.shooterViewID = (ushort)(((uint)Bytes[2] << 8) + (uint)Bytes[3]);
            hitInfo.shooterNetworkID = Bytes[4];
            hitInfo.victimNetworkID = Bytes[5];
            hitInfo.hitPart = (TankBody)Bytes[6];
            hitInfo.partDamage = (ushort)(((uint)Bytes[7] << 8) + (uint)Bytes[8]);
            hitInfo.totalDamage = (ushort)(((uint)Bytes[9] << 8) + (uint)Bytes[10]);
            hitInfo.victimState = Bytes[11];
            return hitInfo;
        }
    }


    //This class is to store the basic information of a GameObject
    public class PhysicsInfo
    {
        public Vector3 Position
        {
            get; set;
        }

        public Vector3 EularAngles
        {
            get; set;
        }
        public float Scale
        {
            get; set;
        }
        public Vector3 Speed
        {
            get; set;
        }
        public PhysicsInfo(Vector3 _position, Vector3 _rotation)
        {
            Position = _position;
            EularAngles = _rotation;
            Scale = 1;
            Speed = new Vector3(0, 0, 0);
        }

    }

    public class TankInfo : PhysicsInfo
    {
        public Vector3 TurretEularAngles
        {
            get; set;
        }
        public int Health
        {
            get; set;
        }
        public float Protection
        {
            get; set;
        }
        public TankOrder tankOrder;
        public int[] PartHealth = new int[2];
        public float[] PartProtection = new float[2];
        public TankInfo(Vector3 _position, Vector3 _eularAngles,Vector3 _turretEularAngles,int _health,float _protection,int [] _partHealth, float[] _partProtection, TankOrder _order)
            :base(_position,_eularAngles)
        {
            PartHealth = _partHealth;
            PartProtection = _partProtection;
            TurretEularAngles = _eularAngles;
            Health = _health;
            tankOrder = _order;
            _partHealth.CopyTo(PartHealth, 0);
            _partProtection.CopyTo(PartProtection, 0);
        }
    }

    public interface IFetchPhysicsInfo
    {
        PhysicsInfo ReturnInfo();
    }
    public interface IFetchTankInfo:IFetchPhysicsInfo
    {
        TankInfo FetchTankInfo();
    }
    public class PerceptInfo
    {
        public Vector3[] positionPredict = new Vector3[5];
        public int[] health = new int[5];
        public int[] turHeal = new int[5];
        public int[] botHeal = new int[5];
        public GameObject[] attacktank = new GameObject[5];
        public int[] bulletNum = new int[5];
        public int[] hitCount = new int[5];
        public float[] moveDis = new float[5];
        public float[] moveTim = new float[5];

    }
}