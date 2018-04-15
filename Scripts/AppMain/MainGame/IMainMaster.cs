using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.MainGame
{
    using ZF.MainGame.Base;
    public interface IMainMaster
    {
        bool SetBot(Tank aiTank);
        bool DeleteBot(Tank aiTank);
    }
}
