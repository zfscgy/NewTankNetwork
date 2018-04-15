using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.WholeGame
{
    using ZF.MainGame.Stats;
    public class GameInteractionManager
    {
        private HitStat hitStat;
        private List<HitInfo> UnRecordedHits = new List<HitInfo>();
        public void NewHit(HitInfo hitInfo)
        {
            UnRecordedHits.Add(hitInfo);
            hitStat.AddHit(hitInfo);
        }
        
        public List<HitInfo> GetUnRecordedHits()
        {
            return UnRecordedHits;
        }

    }
}
