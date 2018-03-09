using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ZF.Configs
{
    [System.Serializable]
    public class TankNetworkComponents
    {
        public bool isSet = false;
        public Transform referenceTankBody;
        public Transform referenceTurret;
        public void Set(Communication.Syncer syncer)
        {
            referenceTankBody = syncer.transform_1;
            referenceTurret = syncer.transform_2;
            isSet = true;
        }
    }
}
