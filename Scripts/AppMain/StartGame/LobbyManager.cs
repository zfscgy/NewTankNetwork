﻿using System;
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

        public int ConnectToPhotonServer(string hostAddress, int port)
        {
            if (!PhotonNetwork.connected)
            {
                if(PhotonNetwork.ConnectToMaster(hostAddress, port, Global.photonAppKey, "1"))
                {
                    GameState.isOnline = true;
                    GameState.mode = GameMode.inLobby;
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

        public int PlayOffline()
        {
            GameState.mode = GameMode.inOfflineGame;
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
                roomController.GetComponent<Server.ServerController>().enabled = true;
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
                roomManager.OnJoinedRoom();
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
                RoomInfos[i] = roominfo.Name + " " + roominfo.PlayerCount.ToString() + "/" + roominfo.MaxPlayers.ToString();
                i++;
            }
            return RoomInfos;
        }
    }
}