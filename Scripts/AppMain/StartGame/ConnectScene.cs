using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ZF.StartGame
{
    enum State
    {
        Offline,
        InLobby,
        InRoom,
    }
    public class ConnectScene:MonoBehaviour
    {
        #region ShowInUnitySlots
        //UI Elements
        public InputField inputAddress;
        public InputField inputPort;
        public Text textConnectionInfo;
        public Button buttonConnect;
        public InputField inputPlayerName;
        public Button buttonRename;
        public InputField inputRoomName;
        public Button buttonStartServer;
        public Text textState;
        public GameObject[] RoomCards;
        public Text[] RoomNameTexts;
        public Text[] RoomNumTexts;

        public GameObject panelLeft;
        public GameObject panelMid;
        public GameObject panelRight;
        public string[] RoomInfos;
        //Scripts
        public LobbyManager lobbyManager;
        #endregion

        private State state;

        #region MonoBehavior Callbacks


        private void Awake()
        {

        }
        private void Start()
        {
            RoomNameTexts = new Text[RoomCards.Length];
            RoomNumTexts = new Text[RoomCards.Length];
            for(int i = 0;i< RoomCards.Length; i++)
            {
                RoomNameTexts[i] = RoomCards[i].transform.Find("Text_Roomname").GetComponent<Text>();
                RoomNumTexts[i] = RoomCards[i].transform.Find("Text_PlayerNumber").GetComponent<Text>();
            }
        }
        private void OnGUI()
        {
            if (state == State.InLobby)
            {
                UpdateRoomList();
            }
        }
        #endregion

        public void OnClick_ButtonConnect()
        {
            TryConnectToPhotonServer();
        }
        public void OnClick_ButtonRename()
        {
            lobbyManager.RenamePlayer(inputPlayerName.text);
        }
        public void OnClick_ButtonPlayOffline()
        {
            if(state == State.InRoom)
            {
                textState.text = "请先退出房间！";
                return;
            }
            Global.GameState.mode = Global.GameMode.inOfflineGame;
            lobbyManager.PlayOffline();
        }
        public void OnClick_ButtonStartServer()
        {
            TryStartAsServer();
        }
        public void OnClick_ButtonJoinRoom(int id)
        {
            Debug.Log("Trying to join id:" + id);
            TryJoinRoom(RoomInfos[id].Split(' ')[0]);
        }



        private void UpdateRoomList()
        {
            RoomInfos = lobbyManager.GetRoomList();
            if (RoomInfos == null)
            {
                return;
            }
            for(int i = 0; i< RoomCards.Length; i++)
            {
                if(i < RoomInfos.Length)
                {
                    if (RoomCards[i].activeSelf == false)
                    {
                        RoomCards[i].SetActive(true);
                    }
                    RoomNameTexts[i].text = RoomInfos[i].Split(' ')[0];
                    RoomNumTexts[i].text = RoomInfos[i].Split(' ')[1];
                }
                else
                {
                    if (RoomCards[i].activeSelf == true)
                    {
                        RoomCards[i].SetActive(false);
                    }
                }                
            }
        }
        private bool TryConnectToPhotonServer()
        {
            int portNumber;
            if (!Int32.TryParse(inputPort.text, out portNumber))
            {
                textConnectionInfo.text = "端口号不合法，请重新输入端口号！";
                return false;
            }
            int re = lobbyManager.ConnectToPhotonServer(inputAddress.text, portNumber);
            if(re == -1)
            {
                textConnectionInfo.text = "连接失败，请检查地址是否正确！";
                return false;
            }
            textConnectionInfo.text = "连接成功！\n" + inputAddress.text + ":" + inputPort.text;
            state = State.InLobby;
            buttonConnect.enabled = false;
            
            return true;
        }
        private bool TryStartAsServer()
        {
            if(state != State.InLobby)
            {
                textState.text = "不在大厅中，无法作为服务器启动！";
                return false;
            }
            if(inputRoomName.text == null || inputRoomName.text == "")
            {
                textState.text = "请输入房间名！";
            }
            int re = lobbyManager.StartServer(inputRoomName.text);
            if(re == -1)
            {
                textState.text = "未连接，请重新连接！";
                return false;
            }
            else if(re == 0)
            {
                textState.text = "已经作为服务器启动！";
            }
            buttonStartServer.enabled = false;
            panelMid.SetActive(false);
            panelRight.SetActive(true);
            return true;
        }
        private bool TryJoinRoom(string roomName)
        {
            if(roomName == null)
            {
                textState.text = "房间名不能为空！";
                return false;
            }
            if(lobbyManager.JoinRoom(roomName))
            {
                textState.text = "成功加入房间！";
                state = State.InRoom;
                //panelLeft.SetActive(false);
                panelMid.SetActive(false);
                panelRight.SetActive(true);
                return true;
            }
            else
            {
                textState.text = "房间无法加入！";
                return false;
            }
        }

    }
}
