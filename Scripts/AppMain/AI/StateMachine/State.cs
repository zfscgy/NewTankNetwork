using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.AI.StateMachine
{

    public abstract class State
    {
        protected StateInfo stateInfo;
        public virtual void Init(StateInfo _stateInfo)
        {
            stateInfo = _stateInfo;
        }

        public virtual void Start()
        {

        }
        /// <summary>
        /// Do the action
        /// Conventionally it will be called once in every update
        /// </summary>
        public virtual void Action()
        {

        }
        /// <summary>
        /// Decide whether it should jump out of this state or remain
        /// </summary>
        /// <returns>The state it jumps to (if is 'this' then remain)</returns>
        public virtual State Decide()
        {
            return this;
        }
    }
}
