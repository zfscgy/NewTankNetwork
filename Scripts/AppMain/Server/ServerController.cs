using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ZF.Server
{

    public class ServerController:Photon.MonoBehaviour
    {
        private int[] RoomSeats = new int[Global.Global.playerPerRoom];
        
        private void Start()
        {
            for(int i =0;i<RoomSeats.Length;i++)
            {
                RoomSeats[i] = -1;
            }
            this.enabled = false;
        }

        [PunRPC]
        public bool RPCAddPlayerToServer(int id)
        {
            for(int i =0;i< RoomSeats.Length;i++)
            {
                if(RoomSeats[i]== -1)
                {
                    RoomSeats[i] = id;
                    return true;
                }
            }
            photonView.RPC("RPCReceiveSeatsInfo",PhotonTargets.All, RoomSeats);
            return false;
        }
        [PunRPC]
        public bool RPCChooseSeat(int playerID,int targetSeat)
        {
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
            return false;
        }
    }
}
