using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtils.StateMachine{
    public class StateStack<T> {
        T owner;

        public State<T> CurrentState{get; private set;}
        public Stack<State<T>> Stack {get; private set;}

        public StateStack(T owner){
            this.owner = owner;
            Stack = new Stack<State<T>>();
        }

        public void Push(State<T> state, ArrayList data = null){
            Stack.Push(state);
            CurrentState = state;
            CurrentState.Enter(owner, data);
        }

        public void Pop(){
            Stack.Pop();
            CurrentState.Exit(owner);
            CurrentState = Stack.Peek();
        }

        public void Update(){
            CurrentState?.Update(owner);
        }

        public void ChangeState(State<T> state, ArrayList data = null){
            if (CurrentState != null){
                Stack.Pop();
                CurrentState.Exit(owner);
            }
            Stack.Push(state);
            CurrentState = state;
            CurrentState.Enter(owner, data);
        }
    }
}