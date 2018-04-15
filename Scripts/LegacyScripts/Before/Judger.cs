using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZF;

//
//this script will only be executed in the master client to judge which team is occpying the target place
//

public class Judger : Photon.MonoBehaviour {
    public Flag ownedTeam;
    public Text OccupiedText;
    private float passedTime;
    private bool isOccupied = false;
    public float occupationFinishedTime = 60f;
    public List<TankControl> Enemies = new List<TankControl>();
    private int enemyCount = 0;  //is equal to Enemies.Count
	// Use this for initialization
	void Start () {
        OccupiedText.enabled = false;
	}

    #region Photon.Monobehavior Callbacks
    // Update is called once per frame    
    private float remainTime;
    void Update () {
        if(!photonView.isMine)
        {
            //if isn't master, then use the received data
            remainTime = receivedRemainTime;
            return;
        }
        //Otherwise, calculate the remain time
        //Only when isOccupied is true
		if(isOccupied)
        {
            if(passedTime > occupationFinishedTime)
            {
                FinishedOccupation();
            }
            //More enemies occupying the base, More quick they win
            passedTime += enemyCount * Time.deltaTime;
            remainTime = (occupationFinishedTime - passedTime) / enemyCount;
        }
	}

    void OnGUI()
    {
        //Only show text when isOccupied
        if (isOccupied)
        {
            OccupiedText.text ="Occupied:" + ((int)remainTime).ToString();
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        Debug.Log("Detect Entered: " + collider.name);
        //Just detect the tank's bottom, in order to prevent the same tank trigger twice.
        if (collider.name != "Bottom")
        {
            return;
        }
        TankControl enemy = collider.transform.parent.parent.GetComponent<TankControl>();
        if (enemy.gameObject.tag == "Player")
        {
            //Get intruder's flag, if isn't enemy, we don't need to count
            ZF.Flag flag = enemy.flag;
            Debug.Log("Intruder Flag:" + flag);
            if (flag != ownedTeam)
            {
                Enemies.Add(enemy);
                OneEnemyEntered();
            }
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        Debug.Log("Detect Exit: " + collider.name);
        if (collider.name != "Bottom")
        {
            return;
        }
        TankControl enemy = collider.transform.parent.parent.GetComponent<TankControl>();
        if (enemy.gameObject.tag == "Player")
        {
            if (Enemies.Contains(enemy))
            {
                Enemies.Remove(enemy);
                OneEnemyLeft();
            }
            else
            {
                //throw new System.Exception("Collider fault");
            }
        }
    }

    private float receivedRemainTime = 0f;
    //Sending Counter text via photonview 
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("Serialize View Called");
        if(stream.isWriting)
        {
            stream.SendNext(remainTime);
        }
        else
        {
            receivedRemainTime = (float)stream.ReceiveNext();
        }
    }
    #endregion
    #region Public Methods
    /// <summary>
    /// These two RPC calls is only for setting the text
    /// Because clients get variable remainTime from stream, do not need to calculate
    /// </summary>
    [PunRPC]
    public void RPCStartCounting()
    {
        Debug.Log("RPCStartCounting Called");
        passedTime = 0.0f;
        isOccupied = true;
        OccupiedText.enabled = true;
    }
    [PunRPC]
    public void RPCEndCounting()
    {
        Debug.Log("RPCEndCounting Called");
        isOccupied = false;
        OccupiedText.enabled = false;
    }
    public void FinishedOccupation()
    {
        photonView.RPC("RPCEndCounting", PhotonTargets.All);
        GameObject.Find("DNTD_WholeGameManager").GetComponent<WholeGameManager>().OneSideWins();
    }
    [PunRPC]
    public void RPCDeadInform(int id)
    {
        TankControl deadTank = Enemies.Find(Tank => (Tank.playerID == id));
        //The dead tank is not in the occupation area
        if(deadTank == null)
        {
            return;
        }
        Enemies.Remove(deadTank);
        OneEnemyLeft();
    }
    #endregion
    #region Private Methods


    private void OneEnemyEntered()
    {
        if(Enemies.Count == 1)
        {
            photonView.RPC("RPCStartCounting", PhotonTargets.All);
        }
        enemyCount++;
    }
    private void OneEnemyLeft()
    {
        if (Enemies.Count == 0)
        {
            photonView.RPC("RPCEndCounting", PhotonTargets.All);
        }
        enemyCount--;
 
    }
    #endregion
}
