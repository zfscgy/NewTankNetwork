using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ZF.MainGame.Stats
{
    public struct HitInfo
    {
        public byte attackerSeat;
        public byte victimSeat;
        public byte totalDamage;
        public Base.TankPart hitPart;
        public byte partDamage;
        public byte killed;
        public Vector3 position;
    }
    public class HitStat
    {
        private List<HitInfo> HitList = new List<HitInfo>();
        public void AddHit(HitInfo hitInfo)
        {
            HitList.Add(hitInfo);
        }
    }
}
