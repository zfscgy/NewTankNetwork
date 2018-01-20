using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainGameController : Photon.MonoBehaviour {
    public TankControl localPlayer;
    public List<TankControl> AllPlayers = new List<TankControl>();
    public RadarControl radarControl;

    //For UI
    public Text[] TeamPoints;
    public Text FpsText;
    private WholeGameManager manager;
    private float occupationStartTime;


    private void Awake()
    {
        for (int i = 0; i <= 11; i++)
        {
            Physics.IgnoreLayerCollision(9, i, false); // Reset settings.
            Physics.IgnoreLayerCollision(11, i, false); // Reset settings.
        }
        Physics.IgnoreLayerCollision(9, 9, true); // Wheels do not collide with each other.
        Physics.IgnoreLayerCollision(9, 13, true); // Wheels do not collide with MainBody.
        for (int i = 0; i <= 11; i++)
        {
            Physics.IgnoreLayerCollision(10, i, true); // Suspensions do not collide with anything.
        }
    }
    // Use this for initialization
    void Start ()
    {
        manager = GameObject.Find("DNTD_WholeGameManager").GetComponent<WholeGameManager>();
        manager.EnterTankField(this);
        Debug.Log(PhotonNetwork.player.ID);
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
    void OnGUI()
    {
        if (Time.frameCount % 32 == 0)
        {
            FpsText.text = "FPS:" + (1.0f / Time.deltaTime).ToString("f2");
        }
    }
    #region RPC Methods
    [PunRPC]
    public void RPCOneTeamStartOccupation(int occupierID)
    {

    }
    [PunRPC]
    public void RPCStopOccupation()
    {

    }
    [PunRPC]
    public void RPCSyncTeamData(int[] teamData)
    {
        for(int i = 0;i < teamData.Length; i++)
        {
            TeamPoints[i].text = teamData[i].ToString();
        }
    }
    [PunRPC]
    public void RPCStopGame()
    {
        Debug.Log("RPCStopGame called");
        localPlayer.StopGame();
    }
    #endregion
    public void ToStatScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("GameStat");
    }
    #region Private Methods 
    private void SyncRadarObjects()
    {
        if (Time.frameCount % 256 == 0)
        {
            foreach (TankControl tank in AllPlayers)
            {
                if (tank != null)
                {
                    if (tank.flag == localPlayer.flag)
                    {
                        if (!radarControl.FriendObjects.Contains(tank.transform) && tank != localPlayer)
                        {
                            radarControl.FriendObjects.Add(tank.transform);
                        }
                    }
                    else if (!radarControl.TrackedObjects.Contains(tank.transform))
                    {
                        radarControl.TrackedObjects.Add(tank.transform);
                    }
                }
            }
        }
    }
    #endregion
    #region Public Methods

    #endregion
}


