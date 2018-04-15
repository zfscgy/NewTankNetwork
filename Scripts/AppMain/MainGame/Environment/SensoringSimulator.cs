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
        public Transform warPoint;
        public void Init(Tank[] _AllTanks)
        {
            enabled = true;
            AllTanks = _AllTanks;
        }
        public void FindEnemyTanksWithinDistance(Vector3 position, float maxDistance, List<Tank> Tanks, int seatID = -1)
        {
            for(int i = 0; i< AllTanks.Length; i++)
            {
                if (AllTanks[i]!= null && !AllTanks[i].body.IsDead() && !Tanks.Contains(AllTanks[i]) &&
                    (AllTanks[i].transform.position - position).magnitude <= maxDistance && AllTanks[i].seatID / 5 != seatID / 5)
                {
                    Tanks.Add(AllTanks[i]);
                }
            }
        }
        public void LoseEnemyTanksWithoutDistance(Vector3 position, float maxDistance, List<Tank> Tanks)
        {
            for (int i = 0; i < AllTanks.Length; i++)
            {
                if (AllTanks[i] != null && Tanks.Contains(AllTanks[i]) &&
                    (AllTanks[i].body.IsDead() || (AllTanks[i].transform.position - position).magnitude > maxDistance)) 
                {
                    Tanks.Remove(AllTanks[i]);
                }
            }
        }
    }
}
