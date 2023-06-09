using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtils.StateMachine
{
    public abstract class State<T>
    {
        public virtual void Enter(T owner, ArrayList data = null){}
        public virtual void Update(T owner){}
        public virtual void Exit(T owner){}
    }
}
