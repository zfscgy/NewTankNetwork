using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.MainGame.Base
{
    interface IMoveable
    {
        Transform GetTransform(); 
        float GetSpeed();
    }
}
