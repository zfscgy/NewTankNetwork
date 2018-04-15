using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using ZF;
public class WaitingController : Photon.PunBehaviour
{
    #region Private Variables
    // 当前房间
    Room thisRoom;
    // 玩家卡片和提示文字
    public PlayerCard[] playercards;
    Text userName;
    Text isReady;
    RawImage panel;
    GameObject roomInfo;
    Text roomName;
    Text roomAnonymous;
    // 特殊成员：自己/房主
    PhotonPlayer me;
    PhotonPlayer master;
    // 房主操作
    GameObject begin;
    GameObject[] pops;
    GameObject quit;
    GameObject TankModel;
    //Whole Game Manager
    public WholeGameManager manager;
    //阵营
    int playerID;
    //The class to pick an index
    //Only useful in master client
    TeamPicker teamPicker;
    #endregion
    #region PunBehaviour CallBacks
    // Use this for initialization
    void Start () {
        // Get UI Elements
        begin = GameObject.Find("/Canvas/buttons/Begin");
        pops = GameObject.FindGameObjectsWithTag("Pop");
        roomInfo = GameObject.Find("/Canvas/Info");
        TankModel = GameObject.Find("/Canvas/Options/Dropdown");
        //Network Variables
        thisRoom = PhotonNetwork.room;
        me = PhotonNetwork.player;
        master = PhotonNetwork.masterClient;

        for (int id = 0; id < 2*Global.MAX_NUMBER_EACH_SIDE; id++)
        {
            playercards[id].id = id;
        }
        SetReady(me, false);
        //初始化坦克模型为普通坦克
        ExitGames.Client.Photon.Hashtable modelProperties = new ExitGames.Client.Photon.Hashtable();
        modelProperties.Add("TankModel", 0);
        PhotonNetwork.player.SetCustomProperties(modelProperties);
        //Find controller Objects
        teamPicker = GameObject.Find("DNTD_WholeGameManager").GetComponent<TeamPicker>();
        manager = GameObject.Find("DNTD_WholeGameManager").GetComponent<WholeGameManager>();
        CallGetIndex();
        SetIndex();
        manager.PlayerID = playerID;
    }

    // Update is called once per frame
    void Update () {
        
    }
    void OnGUI()
    {
        if(manager.GetGameStage()!= GameStage.waiting)
        {
            return;
        }
        foreach (GameObject pop in pops)
        {
            if (PhotonNetwork.isMasterClient)
            {
                pop.SetActive(true);
            }
            else
            {
                pop.SetActive(false);
            }
        }
        // 显示房间信息
        roomName = roomInfo.GetComponentsInChildren<Text>()[0];
        roomAnonymous = roomInfo.GetComponentsInChildren<Text>()[1];
        if (PhotonNetwork.room != null)
        {
            roomName.text = (PhotonNetwork.room == null ? "" : PhotonNetwork.room.Name);
            roomAnonymous.text = (PhotonNetwork.room.IsVisible ? "公开" : "隐藏");
        }
        // 初始化玩家序列
        // 将玩家信息展示在玩家卡片上
        int id;
        for (id = 0; id < 2*Global.MAX_NUMBER_EACH_SIDE; id++)
        {
            playercards[id].used = 0;
        }
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            // 防止玩家离开但还未被删除时报错
            if (!player.AllProperties.ContainsKey("index"))
            {
                continue;
            }
            id = (int)player.AllProperties["index"];
            //Debug.Log("Cached ID " + id);  注释掉防止日志滚屏
            if (id < 0) 
            {
                SetIndex();
                continue;
            }
            playercards[id].used = 1;
            userName =playercards[id].card.GetComponentsInChildren<Text>()[0];
            isReady = playercards[id].card.GetComponentsInChildren<Text>()[1];
            // 是否准备好
            isReady.text = (bool)player.AllProperties["isReady"] ? "准备" : "未准备";
            // 显示自己/房主/玩家昵称
            if (player.Equals(master) && !player.Equals(me))
            {
                userName.text = player.NickName + "/房主";
            }
            else if (!player.Equals(master) && player.Equals(me))
            {
                userName.text = player.NickName + "/自己";
            }
            else if (player.Equals(master)&& player.Equals(me))
            {
                userName.text = player.NickName + "/自己/房主";
                // 不能踢自己
                Transform transform = playercards[id].card.transform.Find("Pop");
                transform.gameObject.SetActive(false);
            }
            else
            {
                userName.text = player.NickName + "/玩家";
            }
        }
        // 没有玩家的卡片置空
        for (id=0; id < 2*Global.MAX_NUMBER_EACH_SIDE; id++)
        {
            if (playercards[id].used == 0)
            {
                Transform transform = playercards[id].card.transform.Find("Pop");
                transform.gameObject.SetActive(false);
                userName = playercards[id].card.GetComponentsInChildren<Text>()[0];
                isReady = playercards[id].card.GetComponentsInChildren<Text>()[1];
                userName.text = "空";
                isReady.text = "";
            }
        }
        // 只有房主显示踢人和开始按钮
        if (!PhotonNetwork.isMasterClient)
        {
            begin.SetActive(false);
        }
    }
    #endregion
    #region Public Methods
    // 设置准备
    public void Ready(GameObject readyInfo)
    {
        if ((bool)me.AllProperties["isReady"])
        {
            SetReady(me, false);
            readyInfo.GetComponent<Text>().text = "准备";
        }
        else
        {
            SetReady(me, true);
            readyInfo.GetComponent<Text>().text = "取消准备";
        }
    }
    // 离开房间
    public void Back()
    {
        CallReturnIndex();
        this.enabled = false;
        PhotonNetwork.LeaveRoom();
    }
    // 开始游戏
    public void Begin()
    {
        // 开始游戏逻辑写在这里
        PhotonNetwork.LoadLevel("tankField");
    }
    // 踢人
    public void Pop(int i)
    {
        PhotonPlayer person = null;
        // 查找按钮的父对象玩家卡片对应的序号
                //根据玩家卡片中关联的ID查找玩家
        foreach(PhotonPlayer p in PhotonNetwork.playerList)
        {
            if((int)p.CustomProperties["index"] == i)
            {
                person = p;
                break;
            }
        }
        // 断开他的连接
        CallReturnIndex();
        playercards[i].used = 0;
        playercards[i].id = -1;
        PhotonNetwork.CloseConnection(person);
    }
    #endregion
    #region Private Methods
    //选择坦克类型
    public void ChooseTank()
    {
        manager.tankModelNumber = TankModel.GetComponent<Dropdown>().value;
    }
    

    //设置自己的阵营和编号
    public void SetIndex()
    {
        Debug.Log("Set ID " + playerID);
        ExitGames.Client.Photon.Hashtable indexHash = new ExitGames.Client.Photon.Hashtable
        {
            { "index", playerID }
        };
        PhotonNetwork.player.SetCustomProperties(indexHash);
        
    }
 
    public void SetReady(PhotonPlayer player, bool isReady)
    {
        ExitGames.Client.Photon.Hashtable playerProperties = PhotonNetwork.player.CustomProperties;
        
        if (playerProperties.ContainsKey("isReady"))
        {
            playerProperties["isReady"] = isReady;
        }
        else
        {
            playerProperties.Add("isReady", isReady);
        }
        player.SetCustomProperties(playerProperties);
    }
    #endregion
    #region Photon.PunBehaviour CallBacks
    // 当玩家进入房间时
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        base.OnPhotonPlayerConnected(newPlayer);
    }
    // 复写方法，当有玩家离开房间时
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        // 刷新房主
        master = PhotonNetwork.masterClient;
/*        if (me.Equals(master))
        {
            // 更新所有UI
            begin.SetActive(true);
            //ReIndex(otherPlayer);
            int leaver = ReturnFlag(otherPlayer);
            Debug.Log("Leaver:" + leaver);
        }*/

    }
    // 复写方法，当自己离开房间时
    public override void OnLeftRoom()
    {
        //销毁自己的序号
        PhotonNetwork.LoadLevel("Launcher");
    }
    #endregion


    //Executed in master client
    //
    // Those Methods use RPC calls to let players to change their team
    // via allocating Indices. 0--4 is for Red team, and 5--9 for blue.
    // Because this class is attached to the waiting scene, we will call methods in another class:
    // TeamPicker, which is attached to DNTD_WholeGameManager, this gameObject is applied DontDestroyOnload
    // and created in the first scene.
    public void CallGetIndex()
    {
        photonView.RPC("RPCGetIndex", PhotonTargets.MasterClient,PhotonNetwork.player.ID, Flag.Red);
    }
    [PunRPC]
    void RPCGetIndex(int actorID,Flag preferredFlag)
    {
        int choosedIndex = teamPicker.AllocateNewIndex(actorID, 5 * (int)preferredFlag);
        photonView.RPC("RPCReceiveNewIndex", PhotonPlayer.Find(actorID), choosedIndex);
    }
    public void CallChangeIndex()
    {
        photonView.RPC("RPCChangeIndex", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
    }
    [PunRPC]
    void RPCChangeIndex(int actorID)
    {
        Debug.Log("RPCChangeIndex called on the master");
        //Find index of current actorID
        int choosedIndex = teamPicker.SwitchIndex(actorID);
        photonView.RPC("RPCReceiveNewIndex", PhotonPlayer.Find(actorID), choosedIndex);
    }
    public void CallReturnIndex()
    {
        photonView.RPC("RPCReturnIndex", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
    }
    [PunRPC]
    void RPCReturnIndex(int actorID)
    {
        teamPicker.SwitchIndex(actorID);
        photonView.RPC("RPCReceiveNewIndex", PhotonPlayer.Find(actorID), -1);
    }
    [PunRPC]
    void RPCReceiveNewIndex(int newIndex)
    {
        Debug.Log("Received new index:" + newIndex);
        playerID = newIndex;
        SetIndex();
    }
}

/* 
   // 封装复用方法，给自己分配一个序号
  // RoomInfo 里的公共变量string "12345678" 来保存序号
  public int GetFlag(Flag preferred)
  {
      Debug.Log("Prefered " + (int)preferred);
      byte[] FlagList;
      int choosedIndex = 0;
      ExitGames.Client.Photon.Hashtable FlagListHash = new ExitGames.Client.Photon.Hashtable();
      if (!PhotonNetwork.room.CustomProperties.ContainsKey("FlagList"))
      {
          FlagList = new byte[maxPlayerNum];
          FlagList[(int)preferred * 5 + 0] = 1;
      }
      else
      {
          FlagList = (byte[])PhotonNetwork.room.CustomProperties["FlagList"];
          int c = 0;
          for (int i = (int)preferred; c < 2; i = (i + 1) % 2) 
          {
              c++;
              for (int j = 0; j < 5; j++) 
              {
                  if (FlagList[i * 5 + j] == 0)
                  {
                      FlagList[i * 5 + j] = 1;
                      choosedIndex = i * 5 + j;
                      c = 2;//To jump out of first layer
                      break;
                  }
              }            
          }
      }
      Debug.Log(FlagList[0].ToString()+ FlagList[1].ToString()+ FlagList[2].ToString()+ FlagList[3].ToString()+ FlagList[4].ToString()+ FlagList[5].ToString()+ FlagList[6].ToString() + FlagList[7].ToString() + FlagList[8].ToString() + FlagList[9].ToString());
      FlagListHash.Add("FlagList", FlagList);
      PhotonNetwork.room.SetCustomProperties(FlagListHash);
      Debug.Log("new ID" + choosedIndex);     
      return choosedIndex;
  } 
  public int ReturnFlag(PhotonPlayer player)
  {
      byte[] FlagList = (byte[])PhotonNetwork.room.CustomProperties["FlagList"];
      FlagList[(int)player.CustomProperties["index"]] = 0;
      int priorID = (int)player.CustomProperties["index"];
      ExitGames.Client.Photon.Hashtable indexHash = new ExitGames.Client.Photon.Hashtable();
      indexHash.Add("index",  -1);
      player.SetCustomProperties(indexHash);

      ExitGames.Client.Photon.Hashtable FlagListHash = new ExitGames.Client.Photon.Hashtable();
      FlagListHash.Add("FlagList", FlagList);
      PhotonNetwork.room.SetCustomProperties(FlagListHash);
      Debug.Log(FlagList[0].ToString() + FlagList[1].ToString() + FlagList[2].ToString() + FlagList[3].ToString() + FlagList[4].ToString() + FlagList[5].ToString() + FlagList[6].ToString() + FlagList[7].ToString() + FlagList[8].ToString() + FlagList[9].ToString());
      Debug.Log("PriorID is " + priorID);
      return priorID;
  }
  public void ChangeFlag()
  {
      byte[] FlagList = (byte[])PhotonNetwork.room.CustomProperties["FlagList"];
      FlagList[playerID] = 0;
      int priorFlag = playerID / 5; 
      Flag preferred = (Flag)((priorFlag + 1) % 2);
      FlagList = (byte[])PhotonNetwork.room.CustomProperties["FlagList"];
      int c = 0;
      int choosedIndex = 0;
      for (int i = (int)preferred; c < 2; i = (i + 1) % 2)
      {
          c++;
          for (int j = 0; j < 5; j++)
          {
              if (FlagList[i * 5 + j] == 0)
              {
                  FlagList[i * 5 + j] = 1;
                  choosedIndex = i * 5 + j;
                  c = 2;//To jump out of first layer
                  break;
              }
          }
      }
      playerID = choosedIndex;      
      SetIndex();
      manager.PlayerID = playerID;
      ExitGames.Client.Photon.Hashtable FlagListHash = new ExitGames.Client.Photon.Hashtable();
      FlagListHash.Add("FlagList", FlagList);
      PhotonNetwork.room.SetCustomProperties(FlagListHash);
      Debug.Log(FlagList[0].ToString() + FlagList[1].ToString() + FlagList[2].ToString() + FlagList[3].ToString() + FlagList[4].ToString() + FlagList[5].ToString() + FlagList[6].ToString() + FlagList[7].ToString() + FlagList[8].ToString() + FlagList[9].ToString());
  }*/
