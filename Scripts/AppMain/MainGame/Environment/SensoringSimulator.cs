using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.MainGame.Environment
{
    using Global;
    using MainGame.Base;
    public class SensoringSimulator:MonoBehaviour
    {
        private Tank[] AllTanks;
        public void Init(Tank[] _AllTanks)
        {
            enabled = true;
            AllTanks = _AllTanks;
        }
        public Tank[] FindTanksWithinDistance(Vector3 position, float maxDistance, int seatID = -1)
        {
            Tank[] Tanks = new Tank[GameState.playerNum];
            int n = 0;
            for(int i = 0; i< GameState.playerNum; i++)
            {
                Debug.Log("Finding Tank:" + i);
                if ((AllTanks[i].transform.position - position).magnitude <= maxDistance && AllTanks[i].seatID != seatID)
                {
                    Tanks[n++] = AllTanks[i];
                }
            }
            return Tanks;
        }
    }
}
