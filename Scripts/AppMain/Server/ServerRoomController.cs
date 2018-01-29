using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ZF.Server
{

    public class ServerRoomController:Photon.MonoBehaviour
    {
        private int[] RoomSeats = new int[Global.Global.playerPerRoom];
        private int playerNum = 0;
        #region Photon.Monobehavior
        private void Start()
        {
            for (int i = 0; i < RoomSeats.Length; i++)
            {
                RoomSeats[i] = -1;
            }
            this.enabled = false;
        } 
        public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            playerNum--;
            if(Global.GameState.mode == Global.GameMode.isServer)
            {
                for (int i = 0; i < RoomSeats.Length; i++)
                {
                    if (RoomSeats[i] == otherPlayer.ID)
                    {
                        RoomSeats[i] = -1;
                    }
                }
                photonView.RPC("RPCReceiveSeatsInfo", PhotonTargets.All, RoomSeats);
            }
        }
        #endregion

        [PunRPC]
        public bool RPCAddPlayerToServer(int id)
        {
            playerNum++;
            Debug.Log("Server RPCAddPlayerToServer, playerID:" + id.ToString());
            for(int i = 0;i< RoomSeats.Length;i++)
            {
                if(RoomSeats[i] == -1)
                {
                    Debug.Log("Allocated One Seat");
                    RoomSeats[i] = id;
                    photonView.RPC("RPCReceiveSeatsInfo",PhotonTargets.All, RoomSeats);
                    return true;
                }
            }
            
            return false;
        }
        [PunRPC]
        public bool RPCChooseSeat(int playerID,int targetSeat)
        {
            Debug.Log("Server RPCChooseSeat, playerID:" + playerID.ToString());
            int currentSeat = -1;
            for (int i = 0; i < RoomSeats.Length; i++)
            {
                if(RoomSeats[i] == playerID)
                {
                    currentSeat = i;
                    break;
                }
            }
            if (RoomSeats[targetSeat] != -1 || currentSeat == -1)
            {
                return false;
            }
            RoomSeats[targetSeat] = playerID;
            RoomSeats[currentSeat] = -1;
            photonView.RPC("RPCReceiveSeatsInfo", PhotonTargets.All, RoomSeats);
            return true;
        }

        public void StartGame()
        {
            Global.GameState.playerNum = playerNum;
            Global.GameState.PlayerIDs = RoomSeats;
            PhotonNetwork.LoadLevel("MainGame");
        }
    }
}
