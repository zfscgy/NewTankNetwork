using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ZF.StartGame
{
    using ZF.Global;
    public class LobbyManager : Photon.PunBehaviour
    {
        public GameObject roomController;

        private void Start()
        {
            PhotonNetwork.autoJoinLobby = true;
        }
        public int ConnectToPhotonServer(string hostAddress, int port)
        {
            if (!PhotonNetwork.connected)
            {
                if(PhotonNetwork.ConnectToMaster(hostAddress, port, Global.photonAppKey, "1"))
                {
                    GameState.isOnline = true;
                    GameState.mode = GameMode.inLobby;
                    GameSettings.serverIPAddress = hostAddress;
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return 1; //Already connected
            }
        }
        public int DisconnectToPhotonServer()
        {
            if(!PhotonNetwork.connected)
            {
                return 1;
            }
            else
            {
                PhotonNetwork.Disconnect();
                return 0;
            }
        }

        public void RenamePlayer(string newName)
        {
            PhotonNetwork.player.NickName = newName;
        }

        public int PlayOffline()
        {
            GameState.mode = GameMode.inOfflineGame;
            SceneManager.LoadScene(Global.mainSceneName);
            return 0;
        }

        public int PlayOfflineAIServer()
        {
            GameState.mode = GameMode.asOfflineAIServer;
            SceneManager.LoadScene(Global.mainSceneName);
            return 0;
        }

        public int StartServer(string roomName)
        {
            if(!PhotonNetwork.connected)
            {
                return -1;
            }
            GameState.mode = GameMode.isServer;
            if (PhotonNetwork.CreateRoom(roomName))
            {
                roomController.GetComponent<Server.ServerRoomController>().enabled = true;
                roomController.GetComponent<RoomManager>().enabled = true;
                return 0;
            }
            return 1;
        }

        public bool JoinRoom(string roomName)
        {
            if(PhotonNetwork.JoinRoom(roomName))
            { 
                GameState.mode = GameMode.inRoom;
                RoomManager roomManager = roomController.GetComponent<RoomManager>();
                roomManager.enabled = true;
                return true;
            }
            return false;
        }

        public string[] GetRoomList()
        {
            if(!PhotonNetwork.insideLobby)
            {
                return null;
            }
            RoomInfo[] roomList = PhotonNetwork.GetRoomList();
            string[] RoomInfos = new string[roomList.Length];
            int i = 0;
            foreach(RoomInfo roominfo in roomList)
            {
                RoomInfos[i] = roominfo.Name + " " + (roominfo.PlayerCount - 1).ToString() + "/" + Global.playerPerRoom.ToString();
                i++;
            }
            return RoomInfos;
        }

        public void SetRecord(bool isRecording)
        {
            GameState.isRecording = isRecording;
        }

        public void Replay(string filename)
        {
            GameState.isRecording = true;
            GameState.saveFilename = filename;
            GameState.mode = GameMode.isPlayBack;
            SceneManager.LoadScene(Global.mainSceneName);
        }
    }
}
