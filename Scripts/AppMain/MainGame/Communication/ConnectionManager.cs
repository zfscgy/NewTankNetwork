using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.Communication
{
    public class ConnectionManager:Photon.MonoBehaviour
    {
        public int GetPing()
        {
            return PhotonNetwork.GetPing();
        }
    }
}
