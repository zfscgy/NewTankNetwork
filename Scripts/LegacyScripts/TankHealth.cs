using UnityEngine;
using UnityEngine.UI;
using ConfigManager;
using ZF;

public class TankHealth : Photon.MonoBehaviour {
    //Read Config
    public XMLConfigReader tankHeathConfigReader;

    //Tank Components
    public int health;
    public float protection;
    
    public TankBodyPart turret = new TankBodyPart("turret");
    public TankBodyPart bottom = new TankBodyPart("bottom");

    //For UI elements
    public Text healthText;
    public Text turretHealthText;
    public Text bottomHealthText;

    #region Photon.MonoBehaviors Callbacks
    // Use this for initialization
    void Start ()
    {
        //Read data from "TankConfig.xml"
        tankHeathConfigReader = new XMLConfigReader();
        tankHeathConfigReader.SetTankHealth("Assets\\Scripts\\TankConfig.xml", "TankHealth", this,
            GameObject.Find("DNTD_WholeGameManager").GetComponent<WholeGameManager>().tankModelNumber);
        if (!photonView.isMine)
        {
            enabled = false;
        }

        //For UI initilization
        healthText = GameObject.Find("Canvas/Panel/TextPanel1/HealthText").GetComponent<Text>();
        turretHealthText = GameObject.Find("Canvas/Panel/TextPanel1/TurretHealthText").GetComponent<Text>();
        bottomHealthText = GameObject.Find("Canvas/Panel/TextPanel1/BottomHealthText").GetComponent<Text>();
        UpdateHealthText();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!photonView.isMine)
        {
            return;
        }
	}
    #endregion

    #region RPC Methods
    [PunRPC]
    public void RPCTakeDamage(byte[] hitInfoBytes)
    {
        Debug.Log("RPC Take Damage Called");
        HitInfo hitInfo = HitInfo.BytesToHitInfo(hitInfoBytes);
        TakeDamage(hitInfo);
    }
    public void TakeDamage(HitInfo shooterSideInfo)
    {
        if(health <= 0)
        {
            return;
        }
        TankBodyPart damagedPart;
        TankBody damagedBody = shooterSideInfo.hitPart;
        switch(damagedBody)
        {
            case TankBody.Bottom: damagedPart = bottom;break;
            case TankBody.Turret: damagedPart = turret;break;
            default:damagedPart = null;break;
        }
        int partDamage = 0;
        int overallDamage = 0;
        if(damagedPart!=null)
        {
            overallDamage = (int)((float)(damagedPart.TakeDamage(shooterSideInfo.shooterDamage,out partDamage))/protection);
        }
        int realDamage = (overallDamage > health)?health:overallDamage;
        health -= realDamage;
        //
        int shooterViewID = shooterSideInfo.shooterViewID;
        shooterSideInfo.hitPart = damagedBody;
        shooterSideInfo.partDamage = (ushort)partDamage;
        shooterSideInfo.totalDamage = (ushort)realDamage;
        shooterSideInfo.victimNetworkID = (byte)photonView.ownerId;
        shooterSideInfo.victimState = (byte)health;

        PhotonPlayer shooterOwner = PhotonPlayer.Find(shooterSideInfo.shooterNetworkID);
        PhotonView shooterView = PhotonView.Find(shooterSideInfo.shooterViewID);

        //Recall the shooter
        shooterView.RPC("RPCHitOneTank", shooterOwner,shooterSideInfo.ToBytes());


        //For UI operation
        UpdateHealthText();
        if (health <= 0)
        {
            Death();
        }
    }

    #endregion

    private void Death()
    {
        //PhotonNetwork.Destroy(transform.parent.gameObject.GetPhotonView());
        TankControl tank = transform.parent.GetComponent<TankControl>();
        tank.gameStage = GameStage.dead;
        GameObject.Find("DNTD_WholeGameManager").GetPhotonView().RPC("RPCPlayerDeathInform", PhotonTargets.MasterClient,tank.playerID);
        
    }

    private void UpdateHealthText()
    {
        healthText.text = " Heath:" + health.ToString() + "/" + protection.ToString("F1");
        turretHealthText.text = " Turret:" + turret.health.ToString() +"/"+ turret.protection.ToString("F1");
        bottomHealthText.text = " Bottom:" + bottom.health.ToString() + "/" + bottom.protection.ToString("F1");
    }
}

/*
 
    private void ChooseTankHealth()
    {
        ExitGames.Client.Photon.Hashtable playerProperties = PhotonNetwork.player.AllProperties;
        //ordinaryTank 
        if ((int)playerProperties["TankModel"] == 0)
        {
            health = 100;
            protection = 5;
            turret.health = 100;
            turret.protection = 2;
            bottom.health = 100;
            bottom.protection = 1;
            Debug.Log("TankControl.ChooseTankHealth.ordinaryTank0");
        }
        //fastTank
        if ((int)playerProperties["TankModel"] == 1)
        {
            health = 20;
            protection = 1;
            turret.health = 20;
            turret.protection = 2;
            bottom.health = 20;
            bottom.protection = 1;
            Debug.Log("TankControl.ChooseTankHealth.fastTank1");
        }
        //heavyTank
        if ((int)playerProperties["TankModel"] == 2)
        {
            health = 1000;
            protection = 10;
            turret.health = 100;
            turret.protection = 2;
            bottom.health = 100;
            bottom.protection = 1;
            Debug.Log("TankControl.ChooseTankHealth.heavyTank2");
        }
    }*/
