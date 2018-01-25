using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ZF.StartGame
{
    public class RoomManager:Photon.MonoBehaviour
    {
        public Text textRoomName;
        public Button buttonExitRoom;
        public Button buttonStartGame;
        public GameObject[] RoomSeats;
        public Text[] RoomSeatTexts;
        private int[] SeatsToID;
        private int playerID;

        public ConnectScene connectScene;
        public Server.ServerRoomController serverRoomController;
        #region Photon.Monobehavior Callbacks        
        private void Start()
        {
            RoomSeatTexts = new Text[RoomSeats.Length];
            for(int i = 0;i < RoomSeats.Length;i++)
            {
                RoomSeatTexts[i] = RoomSeats[i].GetComponentInChildren<Text>();
            }
            if(Global.GameState.mode != Global.GameMode.isServer)
            {
                buttonStartGame.gameObject.SetActive(false);
            }
            PhotonNetwork.automaticallySyncScene = true;
        }
        private void Update()
        {
            if(Global.GameState.mode == Global.GameMode.inRoom)
            {
                //Every 32 frames sync player names in room's seats
                if(Time.frameCount % 32 == 0)
                {
                    Debug.Log("Update room players routinely.");
                    UpdateRoomPlayers();
                }
            }
        }
        private void OnJoinedRoom()
        {
            if (ZF.Global.GameState.mode == Global.GameMode.inRoom)
            {
                Debug.Log("JoinRoom entered in client, trying RPCAddPlayerToServer");
                photonView.RPC("RPCAddPlayerToServer", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
            }
            textRoomName.text = PhotonNetwork.room.Name;
        }
        private void OnLeftRoom()
        {
            connectScene.OnClick_ButtonConnect();
            enabled = false;
            
        }
        #endregion
        #region UI Events
        public void OnClick_ButtonExitRoom()
        {
            Global.GameState.mode = Global.GameMode.inLobby;
            PhotonNetwork.LeaveRoom();
            transform.parent.gameObject.SetActive(false);
            connectScene.panelMid.SetActive(true);
        }
        public void OnClick_ButtonStartGame()
        {
            Global.GameState.stage = Global.GameStage.preparing;
            serverRoomController.StartGame();
        }
        public void OnClick_ChangeSeat(int targetSeat)
        {
            Debug.Log("Onclick_ChangeSeat entered.");
            if (Global.GameState.mode != Global.GameMode.isServer)
            {
                photonView.RPC("RPCChooseSeat", PhotonTargets.MasterClient, PhotonNetwork.player.ID, targetSeat);
            }
        }
        #endregion

        //Update names in seats.
        public void UpdateRoomPlayers()
        {
            if(SeatsToID == null)
            {
                return;
            }
            for(int i = 0;i<SeatsToID.Length;i++)
            {
                if (SeatsToID[i] != -1)
                {
                    PhotonPlayer player = PhotonPlayer.Find(SeatsToID[i]);
                    if (player != null)
                    {
                        RoomSeatTexts[i].text = PhotonPlayer.Find(SeatsToID[i]).NickName;
                        if (SeatsToID[i] == PhotonNetwork.player.ID)
                        {
                            RoomSeatTexts[i].text += "(我)";
                        }
                    }
                    else
                    {
                        RoomSeatTexts[i].text = "空闲";
                    }
                }
                else
                {
                    RoomSeatTexts[i].text = "空闲";
                }
            }
        }

        //Receive Seat Table from server
        [PunRPC]
        void RPCReceiveSeatsInfo(int[] roomSeats)
        {
            SeatsToID = roomSeats;
            UpdateRoomPlayers();
        }
    }
}
