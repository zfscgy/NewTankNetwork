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
        public GameObject[] RoomSeats;
        public Text[] RoomSeatTexts;
        private int[] SeatsToID;
        private int playerID;


        private void Start()
        {
            RoomSeatTexts = new Text[RoomSeats.Length];
            for(int i = 0;i < RoomSeats.Length;i++)
            {
                RoomSeatTexts[i] = RoomSeats[i].GetComponentInChildren<Text>();
            }
            enabled = false;
        }
        public void OnJoinedRoom()
        {
            photonView.RPC("RPCAddPlayerToServer", PhotonTargets.MasterClient, photonView.ownerId);
        }
        public void UpdateRoomPlayers()
        {
            for(int i = 0;i<SeatsToID.Length;i++)
            {
                RoomSeatTexts[i].text = PhotonPlayer.Find(SeatsToID[i]).NickName;
            }
        }

        [PunRPC]
        void RPCReceiveSeatsInfo(int[] roomSeats)
        {
            SeatsToID = roomSeats;
            UpdateRoomPlayers();
        }
    }
}
