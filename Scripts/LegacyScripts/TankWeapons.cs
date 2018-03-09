using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ConfigManager;
using ZF;

public class TankWeapon : Photon.MonoBehaviour {
    //Read Config
    public XMLConfigReader weaponConfigReader;

    //Tank Components
    public Transform gun;
    public int weaponNum = 1;
    public Weapon[] Weapons;
    public int currentWeapon = 0;
    public float amplification = 1f;

    public int totalHit = 0;
    //The totalDamage this tank outputs
    public int totalOutputDamage = 0;
    public int totalKill = 0;

    private List<HitInfo> HitInfos = new List<HitInfo>();

    //For UI elements
    public Text[] WeaponTexts = new Text[3];
    public Text OutputText;
	// Use this for initialization
	void Start () {
        //Read data from "TankConfig.xml"
        weaponConfigReader = new XMLConfigReader();
        weaponConfigReader.SetTankWeapons("Assets\\Scripts\\TankConfig.xml", "TankWeapons", this,
            GameObject.Find("DNTD_WholeGameManager").GetComponent<WholeGameManager>().tankModelNumber);
        //Find UI elements
        for (int i =0;i<weaponNum;i++)
        {
            WeaponTexts[i] = GameObject.Find("Canvas/Panel/TextPanel1/WeaponText" + i.ToString()).GetComponent<Text>();
        }
        OutputText = GameObject.Find("Canvas/Panel/TextPanel1/OutputText").GetComponent<Text>();
        UpdateWeaponText();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    #region Public Methods
    public void Shoot()
    {
        if(!Weapons[currentWeapon].GetOne())
        {
            return;
        }
        Vector3 bulletPosition = gun.transform.position + 4.0f * gun.transform.forward;
        int damage = (int)(Weapons[currentWeapon].damage * amplification);
        PhotonNetwork.Instantiate
            (
                Weapons[currentWeapon].bulletPrefab.name, bulletPosition, gun.rotation, 0,
                new object[]
                {   photonView.viewID,GetComponent<Rigidbody>().velocity,damage }
                // 0 shooterView ID   1 velocity                        2damage
            );
        //UI Operation
        UpdateWeaponText();
    }

    [PunRPC]
    public void RPCHitOneTank(byte[] hitInfoBytes)
    {
        Debug.Log("RPCHitOneTank called");
        HitInfo hitInfo = HitInfo.BytesToHitInfo(hitInfoBytes);
        HitOneTank(hitInfo);
    }
    public void HitOneTank(HitInfo hitInfo)
    {
        totalHit++;
        HitInfos.Add(hitInfo);        
        totalOutputDamage += hitInfo.totalDamage;
        totalKill += (hitInfo.victimState == 0) ? 1 : 0;
        //UI Operation
        UpdateWeaponText();
    }
    public void UploadGameStat()
    {
        ExitGames.Client.Photon.Hashtable statHash = new ExitGames.Client.Photon.Hashtable();
        statHash.Add("GameStat", new int[] { totalHit, totalOutputDamage, totalKill });
        PhotonNetwork.player.SetCustomProperties(statHash);
    }
    #endregion

    private void UpdateWeaponText()
    {
        for(int i = 0;i<weaponNum;i++)
        {
            WeaponTexts[i].text = " " + Weapons[i].name + ":" + Weapons[i].number;
        }
        OutputText.text = " Total Output:" + totalOutputDamage.ToString();
    }
}

/*
 
    private void ChooseTankWeapon()
    {
        ExitGames.Client.Photon.Hashtable playerProperties = PhotonNetwork.player.AllProperties;
        //ordinaryTank 
        if ((int)playerProperties["TankModel"] == 0)
        {
            Weapons[currentWeapon].number = 100;
            Weapons[currentWeapon].damage = 5;
            Weapons[currentWeapon].name = "ordinary shells";
            Debug.Log("TankControl.ChooseTankWeapon.ordinaryTank0");
        }
        //fastTank
        if ((int)playerProperties["TankModel"] == 1)
        {
            Weapons[currentWeapon].number = 500;
            Weapons[currentWeapon].damage = 2;
            Weapons[currentWeapon].name = "Bursts of shells";
            Debug.Log("TankControl.ChooseTankWeapon.fastTank1");
        }
        //heavyTank
        if ((int)playerProperties["TankModel"] == 2)
        {
            Weapons[currentWeapon].number = 20;
            Weapons[currentWeapon].damage = 30;
            Weapons[currentWeapon].name = "super shells";
            Debug.Log("TankControl.ChooseTankWeapon.heavyTank2");
        }
    }*/
