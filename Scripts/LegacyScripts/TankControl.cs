using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ConfigManager;
using ZF;
public class TankControl : Photon.MonoBehaviour,IFetchTankInfo
{
    //Read Config
    public XMLConfigReader TankConfigReader;

    //Tank Components   
    //Physics:
    public Rigidbody thisRigidbody;
    public Transform turret;
    public Transform gun;
    public Camera myCamera;
    public Wheel[] tankWheels;
    //Custom
    private TankHealth tankHealth;
    //For Displaying tank
    public TextMesh tankName;
    public GameObject radarPoint;


    //Entering the Game

    private WholeGameManager manager;
    public GameStage gameStage = GameStage.preparing;

    //UI elements
    public Text counterText;
    //Display
    public Texture2D tankAim;
    //UI for the player
    public Texture2D shootingBar;
    //for control crosshair
    public int cameraState = 0;

    //Tank Parameters for control
    //Shooting

    public float shootInterval;    
    //Controlling Paras
    public float turretYSpeed;
    public float gunXSpeed;
    public float gunXMax;
    public float gunXMin;
    //For motion 
    //Modified 2017.9.26 for replacing the BaseMove() with prior functions because 
    //changed the wheels and tracks
    public float maxTorque;
    //This parameter is the torque difference between one side and the other
    public float maxTurnDiff;
    public float maxSpeed;
    public float autoParkingBrakeVelocity = 0.5f;
    public bool isParkingBrake;
    public float autoParkingBrakeLag = 0.5f;
    //The acceleration is the torque increasement in a fixedDelta time
    public float acceleration;
    //The speed of wheel rotating
    public float leftRate;
    public float rightRate;


    public TankOrder currentOrder = new TankOrder();
    private TankOrder overrideOrder = new TankOrder();
    public bool isOverride = false;

    //Player Information
    public int playerID = -1;
    public Flag flag;




    #region Photon.MonoBehavior Callbacks
    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        manager = GameObject.Find("DNTD_WholeGameManager").GetComponent<WholeGameManager>();
        //Self Initilization
        tankName.text = photonView.owner.NickName;
        tankHealth = GetComponentInChildren<TankHealth>();
        //Read data from "TankConfig.xml"
        TankConfigReader = new XMLConfigReader();
        TankConfigReader.SetTankControl("Assets\\Scripts\\TankConfig.xml", "TankControl",this,
            manager.tankModelNumber);
        //Get flag from PhotonPlayer's cache
        playerID = (int)photonView.owner.CustomProperties["index"];
        flag = (Flag)(playerID/ Global.MAX_NUMBER_EACH_SIDE);
        //Display 3DText name and Radar Point
        tankName.GetComponent<Renderer>().material.color = (flag == Flag.Red) ? Color.red : Color.blue;
        radarPoint.GetComponent<SpriteRenderer>().color = (flag == Flag.Red) ? Color.red : Color.blue;

        GameObject mainController = GameObject.Find("GameController");
        //Register in GameController
        mainController.GetComponent<MainGameController>().AllPlayers.Add(this);
        //Register in the RadarController Dictionary
        mainController.GetComponent<RadarControl>().tankDict.Add(photonView.ownerId, this);
        if (!photonView.isMine)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            //Only local player can have camera
            Flag masterFlag = (Flag)(manager.PlayerID / 5);
            if(flag == masterFlag)
            {
                mainController.GetComponent<RadarControl>().FriendObjects.Add(transform);
            }
            else
            {
                mainController.GetComponent<RadarControl>().TrackedObjects.Add(transform);
            }
            myCamera.GetComponent<Camera>().enabled = false;
            //GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            //Disactive the display object
            tankName.gameObject.SetActive(false);
            //Register in GameController
            mainController.GetComponent<MainGameController>().localPlayer = this;
            mainController.GetComponent<RadarControl>().radar = transform;

            counterText = GameObject.Find("Canvas/UpperTexts/CounterText").GetComponent<Text>();
            enteredTime = Time.time;
        }
    }

    void Update () {
        if(!photonView.isMine)
        {
            ShowName();
            return;
        }
        if(gameStage != GameStage.gaming)
        {
            return;
        }
        //Get input and then order
              
    }
    void FixedUpdate()
    {
        if (!photonView.isMine)
        {
            ShowName();
            return;
        }
        if (gameStage != GameStage.gaming)
        {
            return;
        }
        PlayerControl();  
        OrderTank();
    }
    void OnGUI()
    {
        if (!photonView.isMine)
        {
            return;
        }
        if(gameStage != GameStage.gaming)
        {
            ShowTimeCounter();
            return;
        }
        //Draw the actual point the turret pointing to
        DrawPoint();        
    }
    #endregion
    #region Public Methods
    public bool Override(TankOrder command)
    {
        if(!isOverride)
        {
            return false;
        }
        overrideOrder = command;
        return true;
    }

    public void StopGame()
    {
        Debug.Log("RPC Stop Game Called");
        gameStage = GameStage.ending;
        enteredTime = Time.time;
        counterText.enabled = true;
        GetComponent<TankWeapons>().UploadGameStat();
    }
    #endregion


    #region Private Methods
    private float enteredTime;
    private float waitingTime = 5.0f;
    private void ShowTimeCounter()
    {
        float remainingTime = waitingTime - (Time.time - enteredTime);
        if (remainingTime < 0f)
        {
            if (gameStage == GameStage.preparing)
            {
                gameStage = GameStage.gaming;
                counterText.enabled = false;
            }
            if(gameStage == GameStage.ending)
            {
                gameStage = GameStage.stop;
                GameObject.Find("GameController").GetComponent<MainGameController>().ToStatScene();
            }
            return;
        }
        counterText.text = ((int)remainingTime).ToString();
    }
    private float lastShootTime;
    private void Shoot()
    {
        //若不到发射间隔，则不发射炮弹
        if (Time.time - lastShootTime < shootInterval)
        {
            return;
        }
        //Call the shoot function in TankWeapons
        GetComponent<TankWeapons>().Shoot();
        lastShootTime = Time.time;
    }

    // To get inputs

    private void ControlTurret()
    {
        Ray cameraRay = myCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitPoint;
        Vector3 direction;
        if (Physics.Raycast(cameraRay, out hitPoint, 400.0f, (1 << 8)|(1<<10)))
        {
            direction = gun.transform.InverseTransformVector(hitPoint.point - gun.position);
        }
        else
        {
            direction = gun.transform.InverseTransformVector(myCamera.transform.forward);
        }
        currentOrder.direction = direction;

    }
    private void ControlMotion()
    {
        int direction;
        if (Input.GetKey(KeyCode.W))
        {
            direction = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction = 2;
        }
        else
        {
            direction = 0;
        }
        currentOrder.move = (char)direction;
    }
    private void ControlSteering()
    {
        int direction = 0;
        if (Input.GetKey(KeyCode.A))
        {
            direction = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction = 2;
        }
        else
        {
            direction = 0;
        }
        currentOrder.steer = (char)direction;
    }

    // Inputs are turned into parameters, stored in tankOrder, then pass to base functions
    private void OrderTank()
    {
        if(isOverride)
        {
            currentOrder = overrideOrder;
        }
        BaseMove_1(currentOrder.move,currentOrder.steer);
        BaseRotateTurret(currentOrder.direction);
        //BaseSteer(currentOrder.steer);
    }
    //Combine three Control functions and shooting in one function
    private void PlayerControl()
    {
        //空格发射炮弹S
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            Shoot();
            currentOrder.shoot = 1;
        }
        else
        {
            currentOrder.shoot = 0;
        }
        ControlMotion();
        ControlSteering();
        ControlTurret();
        OrderTank();
    }

    private void ShowName()
    {
        tankName.transform.forward = Camera.main.transform.forward;
    }

    //Base function for rotating turret and gun
    private float[] lastAngles = new float[2] { 0f, 0f };
    private void BaseRotateTurret(Vector3 direction)
    {
        //        
        //        
        float yAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float xAngle = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;
        float rotateX = -xAngle;
        float rotateY = yAngle;
        float turretRotateMax = turretYSpeed * Time.fixedDeltaTime;
        /*if(rotateY>turretRotateMax)
        {
            rotateY = turretRotateMax;
        }
        else if(rotateY < -turretRotateMax )
        {
            rotateY = -turretRotateMax;
        }*/
        rotateY = Mathf.Clamp(rotateY, -turretRotateMax, turretRotateMax);
        float gunUpMax = gunXSpeed * Time.fixedDeltaTime;
        /*if(rotateX > gunUpMax)
        {
            rotateX = gunUpMax;
        }
        else if(rotateX < -gunUpMax)
        {
            rotateX = -gunUpMax;
        }*/
        rotateX = Mathf.Clamp(rotateX, -gunUpMax, gunUpMax);
        rotateX = (rotateX + lastAngles[0]) / 2f;
        lastAngles[0] = rotateX;
        //Debug.Log(rotateX);
        if (Mathf.Abs(rotateX) < 0.05)
        {
            gun.localEulerAngles += new Vector3(rotateX, 0, 0);
        }
        else
        {
            if ((gun.localEulerAngles.x - 360f) > -gunXMax || gun.localEulerAngles.x < -gunXMin)
            {
                gun.localEulerAngles += new Vector3(rotateX, 0, 0);
            }
            if (gun.localEulerAngles.x <= 200f && gun.localEulerAngles.x >= -gunXMin)
            {
                gun.localEulerAngles = new Vector3((-gunXMin - 0.1f), gun.localEulerAngles.y, gun.localEulerAngles.z);
            }
            else if ((gun.localEulerAngles.x - 360f) <= -gunXMax && gun.localEulerAngles.x > 200f)
            {
                gun.localEulerAngles = new Vector3((360f - gunXMax + 0.1f), gun.localEulerAngles.y, gun.localEulerAngles.z);
            }

        }
        //Debug.Log(gun.localEulerAngles);
        rotateY = (rotateY + lastAngles[1]) / 2f;
        lastAngles[1] = rotateY;
        if (Mathf.Abs(rotateY) < 0.1)
        {
            turret.localEulerAngles += new Vector3(0, rotateY, 0);
        }
        else
        {
            turret.localEulerAngles += new Vector3(0, turretYSpeed * rotateY * Time.fixedDeltaTime, 0);
        }


    }
    //base function for steering
    private float speedStepVertical = 0f;
    private float speedStepHorizental = 0f;
    private float lagCount = 0f;
    //The BaseMove method takes two parameters, vertical for forward, horizental for move;
    //The increasement of torque is a continuous process,however,it can jump to zero immediately after we stop pressing key
    //In order to stablize the tank while it is not moving,use autoParkingBrake
    private void BaseMove_1(int vertical, int horizental)
    {
        //Get vertical motion
        if(vertical == 1)
        {
            if (speedStepVertical < 0)
            {
                speedStepVertical = 0f;
            }
            speedStepVertical += acceleration;
        }
        else if(vertical == 2)
        {
            if(speedStepVertical > 0)
            {
                speedStepVertical = 0f;
            }
            speedStepVertical -= acceleration;
        }
        else if(vertical == 0)
        {
            speedStepVertical = 0f;
        }
        speedStepVertical = Mathf.Clamp(speedStepVertical, -1f, 1f);
        //Get horizental motion
        if (horizental == 2)
        {
            speedStepHorizental = 1.0f;
        }
        else if (horizental == 1)
        {
            speedStepHorizental = -1.0f;
        }
        else if (horizental == 0)
        {
            speedStepHorizental = 0f;
        }
        speedStepHorizental = Mathf.Clamp(speedStepHorizental, -maxTurnDiff, maxTurnDiff);
        float speedStepHorizental_1 = Mathf.Sign(speedStepVertical) * speedStepHorizental;
        leftRate = Mathf.Clamp(-speedStepVertical - speedStepHorizental_1, -1.0f - maxTurnDiff, 1.0f + maxTurnDiff);
        rightRate = Mathf.Clamp(speedStepVertical - speedStepHorizental_1, -1.0f - maxTurnDiff, 1.0f + maxTurnDiff);

        ///
        // Auto Parking Brake using 'RigidbodyConstraints'.
        if (leftRate == 0.0f && rightRate == 0.0f)
        {
            float velocityMag = thisRigidbody.velocity.magnitude;
            float angularVelocityMag = thisRigidbody.angularVelocity.magnitude;
            if (isParkingBrake == false)
            {
                if (velocityMag < autoParkingBrakeVelocity && angularVelocityMag < autoParkingBrakeVelocity)
                {
                    lagCount += Time.fixedDeltaTime;
                    if (lagCount > autoParkingBrakeLag)
                    {
                        isParkingBrake = true;
                        thisRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
                    }
                }
            }
            else
            {
                if (velocityMag > autoParkingBrakeVelocity || angularVelocityMag > autoParkingBrakeVelocity)
                {
                    isParkingBrake = false;
                    thisRigidbody.constraints = RigidbodyConstraints.None;
                    lagCount = 0.0f;
                }
            }
        }
        else
        {
            isParkingBrake = false;
            thisRigidbody.constraints = RigidbodyConstraints.None;
            lagCount = 0.0f;
        }
    }


    //For player UI update
    //calculate the point the gun is actually pointing to
    private Vector3 CalculateHitPoint()
    {
        Vector3 hitPoint;
        RaycastHit hit;
        Ray projection = new Ray(gun.position + 6 * gun.forward, gun.forward);
        if (Physics.Raycast(projection, out hit, 500f, (1<<8)|(1<<10)))
        {
            hitPoint = hit.point;
        }
        else
        {
            hitPoint = projection.GetPoint(500f);
        }
        return hitPoint;
    }

    //UI function to draw that actual point on the screen
    private void DrawPoint()
    {
        float barLength = (shootInterval - (Time.time - lastShootTime)) / shootInterval;
        if(barLength < 0)
        {
            barLength = 0;
        }
        int barLen = (int)(128 * barLength);
        Vector3 aim = CalculateHitPoint();
        if (aim != Vector3.zero)
        {
            //获取坦克准心坐标
            Vector3 screenPoint = myCamera.WorldToScreenPoint(aim);
            //绘制坦克准心
            Rect crosshairRect = new Rect(screenPoint.x - tankAim.width / 2, Screen.height - screenPoint.y - tankAim.height / 2, tankAim.width, tankAim.height);
            GUI.DrawTexture(crosshairRect, tankAim);
            if (barLen > 0)
            {
                Rect barRect = new Rect(Screen.width/2 - barLen / 2, Screen.height/2 - shootingBar.height / 2, barLen, shootingBar.height);
                GUI.DrawTexture(barRect, shootingBar);
            }
        }

    }

    #endregion

    #region Interface Realization
    PhysicsInfo IFetchPhysicsInfo.ReturnInfo()
    {
        return new PhysicsInfo(transform.position, transform.eulerAngles);
    }
    TankInfo IFetchTankInfo.FetchTankInfo()
    {
        return new TankInfo(transform.position, transform.eulerAngles, turret.localEulerAngles,
            tankHealth.health, tankHealth.protection,
            new int[2] { tankHealth.turret.health, tankHealth.bottom.health },
            new float[2] { tankHealth.turret.protection, tankHealth.bottom.protection },
            currentOrder
            );        
    }
    #endregion

}


///
/// Prior version
///
/*void ControlTurret()
{
//Debug.Log(turret.localEulerAngles);
int direction = 0;
if (turret == null)
{
    return;
}
if (Input.GetKey(KeyCode.RightArrow))
{
    direction = 4;
}
if (Input.GetKey(KeyCode.LeftArrow))
{
    direction = 3;
}
if (Input.GetKey(KeyCode.UpArrow))
{
    direction = 1;
}
if (Input.GetKey(KeyCode.DownArrow) && turret.localEulerAngles.x > 100)
{
    direction = 2;
}
currentOrder.turretRotate = (char)direction;
}*/
//炮塔旋转底层函数
/*private void BaseRotateTurret(char direction)
{
    switch((int)direction)
    {   //1 上 2 下 3 左 4  右
        case 1:
            if ((turret.localEulerAngles.x - 360) > -turretVerticalMaxium || turret.localEulerAngles.x < 2.0)
            {
                turret.localEulerAngles += new Vector3(-gunXSpeed * Time.fixedDeltaTime, 0, 0);
            }
            break;
        case 2:
            if (turret.localEulerAngles.x > 100)
            {
                turret.localEulerAngles += new Vector3(gunXSpeed * Time.fixedDeltaTime, 0, 0);
            }
            break;
        case 3:
            turret.localEulerAngles += new Vector3(0f, -turretYSpeed * Time.fixedDeltaTime, 0f);
            //turret.localEulerAngles = Vector3.SLerp(turret.localEulerAngles, turret.localEulerAngles += new Vector3(0f, -turretYSpeed * Time.fixedDeltaTime, 0f), 1f);
            break;
        case 4:
            turret.localEulerAngles += new Vector3(0f, turretYSpeed * Time.fixedDeltaTime, 0f);
            break;
    }
}*/

/*
//选择坦克型号
private void ChooseTank()
{
    ExitGames.Client.Photon.Hashtable playerProperties = PhotonNetwork.player.AllProperties;
    //ordinaryTank
    if ((int)playerProperties["TankModel"] == 0)
    {

        shootInterval = 1.5f;
        turretYSpeed = 10f;
        gunXSpeed = 10f;
        gunXMax = 30.0f;
        gunXMin = -10.0f;
        maxRpm = 200;
        motorTorque = 1500.0f;
        brakeTorque = 200.0f;
        steeringSpeed = 5.0f;
        maxSteering = 10.0f;
        steeringBack = 5.0f;
        Debug.Log("TankControl.ChooseTank.ordinaryTank0");
    }
    //fastTank
    if ((int)playerProperties["TankModel"] == 1)
    {
        shootInterval = 0.15f;
        turretYSpeed = 50f;
        gunXSpeed = 50f;
        gunXMax = 80.0f;
        gunXMin = -60.0f;
        maxRpm = 2000;
        motorTorque = 15000.0f;
        brakeTorque = 2000000.0f;
        steeringSpeed = 50.0f;
        maxSteering = 70.0f;
        steeringBack = 30.0f;
        Debug.Log("TankControl.ChooseTank.fastTank1");
    }
    //heavyTank
    if ((int)playerProperties["TankModel"] == 2)
    {
        shootInterval = 10.0f;
        turretYSpeed = 2f;
        gunXSpeed = 2f;
        gunXMax = 30.0f;
        gunXMin = -10.0f;
        maxRpm = 20;
        motorTorque = 15000.0f;
        brakeTorque = 200.0f;
        steeringSpeed = 1.0f;
        maxSteering = 5.0f;
        steeringBack = 5.0f;
        Debug.Log("TankControl.ChooseTank.heavyTank2");
    }
}*/

/*
 * Strange problem in rotate turret
 * now it seems angles can be arbitrary float value 
 *          if ((gun.localEulerAngles.x - 360) > -gunXMax && gun.localEulerAngles.x < -gunXMin)
            {
                gun.localEulerAngles += new Vector3(gunXSpeed * rotateX * Time.fixedDeltaTime, 0, 0);
            }
            if (gun.localEulerAngles.x <= 200 && gun.localEulerAngles.x >= -gunXMin)
            {
                gun.localEulerAngles = new Vector3((-gunXMin - 0.1f), gun.localEulerAngles.y, gun.localEulerAngles.z);
            }
            else if ((gun.localEulerAngles.x - 360) <= -gunXMax && gun.localEulerAngles.x > 200)
            {
                gun.localEulerAngles = new Vector3((360 - gunXMax + 0.1f), gun.localEulerAngles.y, gun.localEulerAngles.z);
            }
 * 
 */
//2017.9.27
/*
 * Now we use the kawaii-tank projects methods to simulate trank
 * Thanks a lot!
 *
 *     private void BaseSteer(int direction)
   {
       switch(direction)
       {
           case 1:
               tankWheels[0].RightWheel.steerAngle -= steeringSpeed * Time.fixedDeltaTime;
               tankWheels[0].LeftWheel.steerAngle -= steeringSpeed * Time.fixedDeltaTime;
               break;
           case 2:
               tankWheels[0].RightWheel.steerAngle += steeringSpeed * Time.fixedDeltaTime;
               tankWheels[0].LeftWheel.steerAngle += steeringSpeed * Time.fixedDeltaTime;
               break;
           default:
               tankWheels[0].RightWheel.steerAngle -= tankWheels[0].RightWheel.steerAngle *steeringBack* Time.fixedDeltaTime;
               tankWheels[0].LeftWheel.steerAngle -= tankWheels[0].LeftWheel.steerAngle *steeringBack* Time.fixedDeltaTime;
               break;
       }
       if(tankWheels[0].RightWheel.steerAngle>maxSteering || tankWheels[0].LeftWheel.steerAngle>maxSteering)
       {
           tankWheels[0].RightWheel.steerAngle = maxSteering;
           tankWheels[0].LeftWheel.steerAngle = maxSteering;
       }
       if (tankWheels[0].RightWheel.steerAngle < - maxSteering || tankWheels[0].LeftWheel.steerAngle <- maxSteering)
       {
           tankWheels[0].RightWheel.steerAngle = -maxSteering;
           tankWheels[0].LeftWheel.steerAngle = -maxSteering;
       }
   }
       //Base function for motion controlling( move )
    private void BaseTankMove(int direction)
    {
        switch (direction)
        {
            case 1:
                tankWheels[0].RightWheel.motorTorque = motorTorque;
                tankWheels[0].LeftWheel.motorTorque = motorTorque;
                tankWheels[0].RightWheel.brakeTorque = 0;
                tankWheels[0].LeftWheel.brakeTorque = 0;
                break;
            case 2:
                tankWheels[0].RightWheel.motorTorque = -motorTorque;
                tankWheels[0].LeftWheel.motorTorque = -motorTorque;
                tankWheels[0].RightWheel.brakeTorque = 0;
                tankWheels[0].LeftWheel.brakeTorque = 0;
                break;
            default:
                tankWheels[0].RightWheel.motorTorque = 0;
                tankWheels[0].LeftWheel.motorTorque = 0;
                tankWheels[0].RightWheel.brakeTorque = brakeTorque;
                tankWheels[0].LeftWheel.brakeTorque = brakeTorque;
                break;
        }
        if(Mathf.Abs(tankWheels[0].RightWheel.rpm) > maxRpm || Mathf.Abs(tankWheels[0].LeftWheel.rpm) > maxRpm)
        {
            tankWheels[0].RightWheel.motorTorque = 0;
            tankWheels[0].LeftWheel.motorTorque = 0;
        }
    }
*/
