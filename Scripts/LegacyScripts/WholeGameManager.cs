using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class WholeGameManager : Photon.MonoBehaviour {
    private int gameMode = 0;
    private int maxPlayer = 10;
    private int playerID;
    private GameStage gameStage = GameStage.waiting;
    public GameStage GetGameStage()
    {
        return gameStage;
    }
    public int PlayerID
    {
        set
        {
            playerID = value;
        }
        get
        {
            return playerID;
        }
    }
    public int tankModelNumber;

    private int[] PlayerNumEachSide = new int[2] { 0, 0 };
    private MainGameController mainController;
    #region Photon.Monobehavior Callbacks
    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    #endregion

    public void EnterTankField(MainGameController _mainController)
    {
        mainController = _mainController;
        gameStage = GameStage.preparing;
        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            RegisterPlayer((int)player.CustomProperties["index"]);
        }

        ExitGames.Client.Photon.Hashtable sideNumHash = new ExitGames.Client.Photon.Hashtable();
        sideNumHash.Add("SideNum", PlayerNumEachSide);
        PhotonNetwork.room.SetCustomProperties(sideNumHash);
    }
    public void BackToRoom()
    {
        gameStage = GameStage.waiting;
    }
    public int RegisterPlayer(int playerID)
    {
        int i = playerID / Global.MAX_NUMBER_EACH_SIDE;
        PlayerNumEachSide[i] += 1;
        mainController.photonView.RPC("RPCSyncTeamData", PhotonTargets.All, PlayerNumEachSide);
        return i;
    }
    [PunRPC]
    public int RPCPlayerDeathInform(int playerID)
    {
        Debug.Log("RPC PlayerDeathInform called");
        int i = playerID / Global.MAX_NUMBER_EACH_SIDE;
        PlayerNumEachSide[i] -= 1;
        mainController.photonView.RPC("RPCSyncTeamData", PhotonTargets.All, PlayerNumEachSide);
        if(PlayerNumEachSide[i] == 0)
        {
            OneSideWins();
        }
        
        return i;
    }
    public void OneSideWins()
    {        
        mainController.photonView.RPC("RPCStopGame", PhotonTargets.All);
    }

}
public enum GameStage
{
    waiting = 0,
    preparing = 1,
    gaming = 2,
    ending = 3,
    stop = 4,
    dead =5
}