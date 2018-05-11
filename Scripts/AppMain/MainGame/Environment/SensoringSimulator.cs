using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.MainGame.Environment
{
    using Global;
    using MainGame.Stats;
    using MainGame.Base;
    public class SensoringSimulator:MonoBehaviour
    {

        public Configs.EnvironmentConfigs configs;
        private Tank[] AllTanks;
        private float[] TankObviousity;
        public Transform warPoint;
        public void Init(Tank[] _AllTanks)
        {
            enabled = true;
            AllTanks = _AllTanks;
            TankObviousity = new float[AllTanks.Length];
            Singletons.gameInteractionManager.sensoringSimulator = this;
            for(int i = 0; i < TankObviousity.Length; i++)
            {
                TankObviousity[i] = 0.5f;
            }
        }

        float lastSyncTime = -10f;
        void Update()
        {
            if(Time.time - lastSyncTime >= 1f)
            {
                Sync();
            }
            
        }
        private void Sync()
        {
            for(int i = 0; i< TankObviousity.Length;i++)
            {
                TankObviousity[i] *= configs.tankTransparityDecreasing;
            }
        }


        /// <summary>
        /// For players
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maxDistance"></param>
        /// <param name="Tanks"></param>
        /// <param name="seatID"></param>
        public void FindEnemyTanksWithinDistance(Vector3 position, float maxDistance, List<Tank> Tanks, int seatID = -1)
        {
            for(int i = 0; i< AllTanks.Length; i++)
            {
                if (AllTanks[i]!= null && !AllTanks[i].body.IsDead() && !Tanks.Contains(AllTanks[i]) &&
                    (AllTanks[i].transform.position - position).magnitude/ (1 + TankObviousity[i]) <= maxDistance 
                    && AllTanks[i].seatID / 5 != seatID / 5)
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
                    (AllTanks[i].body.IsDead() || (AllTanks[i].transform.position - position).magnitude / (1 + TankObviousity[i]) > maxDistance)) 
                {
                    Tanks.Remove(AllTanks[i]);
                }
            }
        }


        /// <summary>
        /// For info passing
        /// </summary>
        public void OneHitHappened(HitInfo hitInfo)
        {
            TankObviousity[hitInfo.attackerSeat] *= configs.tankTransparityIncreasingByHit;
        }

        public void OneShootHappend(int seatID)
        {
            TankObviousity[seatID] *= configs.tankTransparityIncreasingByShooting;
        }

        ///
        ///
        public void OneShootHappened()
        {

        }
    }
}
