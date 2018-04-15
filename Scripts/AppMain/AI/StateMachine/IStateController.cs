using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.AI.StateMachine
{
    public class StateInfo
    {
        public State[] AvailableStates;
    }
    public interface IStateController
    {
        void Order();
        StateInfo GetInfo();
    }
}
