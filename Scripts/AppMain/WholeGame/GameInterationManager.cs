 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.WholeGame
{
    using ZF.MainGame.Stats;
    using MainGame.Environment;
    public class GameInteractionManager
    {
        private HitStat hitStat;
        private List<HitInfo> UnRecordedHits = new List<HitInfo>();

        public SensoringSimulator sensoringSimulator;
        public void NewHit(HitInfo hitInfo)
        {
            UnRecordedHits.Add(hitInfo);
            hitStat.AddHit(hitInfo);
            sensoringSimulator.OneHitHappened(hitInfo);
        }
        public void NewShoot(int shooter)
        {
            sensoringSimulator.OneShootHappend(shooter);
        }
        public List<HitInfo> GetUnRecordedHits()
        {
            return UnRecordedHits;
        }

    }
}
