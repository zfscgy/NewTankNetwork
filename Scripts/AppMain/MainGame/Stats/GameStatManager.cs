using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.MainGame.Stats
{
    using ZF.Global;
    public class GameStatManager:Photon.MonoBehaviour
    {
        public MainGameUI mainGameUI;

        private Base.Tank localPlayerTank;
        private TankStat[] PlayerStats;

        private void Update()
        {
            if (localPlayerTank != null)
            {
                UpdatePlayerSpeedStat();
            }
        }

        public void Init(Base.Tank[] _PlayerTanks)
        {
            GameState.gameStatManager = this;
            enabled = true;
            SetTanks(_PlayerTanks);
            mainGameUI.Init(PlayerStats[GameState.playerID]);
            localPlayerTank = _PlayerTanks[GameState.playerID];
        }

        public void SetTanks(Base.Tank[] _PlayerTanks)
        {
            PlayerStats = new TankStat[_PlayerTanks.Length];
            for(int i = 0;i<_PlayerTanks.Length;i++)
            {
                if (_PlayerTanks[i] != null)
                {
                    PlayerStats[i] = _PlayerTanks[i].GetStat();
                }
            }
        }

        public void AddTank(Base.Tank newTank)
        {
            PlayerStats[newTank.seatID] = newTank.GetStat();
        }

        public void SerializeAllStat()
        {
            int Length = 5 + GameSettings.n_AmmoKind + GameSettings.n_TankBodyPart;
            byte[] AllStatBytes = new byte[Length * PlayerStats.Length];
            for(int i = 0; i< PlayerStats.Length; i++)
            {
                PlayerStats[i].ToByte5().CopyTo(AllStatBytes, i * 5);
            }
        }
        public TankStat[] GetAllStats()
        {
            return PlayerStats;
        }

        public void CallClientToUpdateStat(int index)
        {
            photonView.RPC("SetPlayerStat", PhotonPlayer.Find(GameState.PlayerIDs[index]), PlayerStats[index].ToBytesAll());
        }
        public void CallUIToUpdate()
        {
            mainGameUI.UpdateGameStatTexts();
        }

        [PunRPC]
        public void SetPlayerStat(byte[] BytesAll)
        {
            PlayerStats[GameState.playerID].SetByBytesAll(BytesAll);
            CallUIToUpdate();
        }
        [PunRPC]
        public void SetAllStat(byte[] AllBytes)
        {
            for(int i = 0; i< PlayerStats.Length; i++)
            {
                PlayerStats[i].SetByBytesAll(AllBytes, i * (5 + GameSettings.n_AmmoKind + GameSettings.n_TankBodyPart));
            }
        }

        /// <summary>
        /// On client, I believe Photon Syncs the rigidBody's speed
        /// </summary>
        public void UpdatePlayerSpeedStat()
        {
            PlayerStats[GameState.playerID].speed = (int)(localPlayerTank.motion.m_rigidbody.velocity.magnitude * 3.6);
        }


        /// <summary>
        /// On game server
        /// </summary>
        public void UploadStat()
        {

        }

        /// <summary>
        /// On game client
        /// </summary>
        public void DownLoadStat()
        {

        }
    }
}
