using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtils.StateMachine{
    public class StateMachine<T>{
        T owner;

        public State<T> CurrentState{get; private set;}
        public Stack<State<T>> StateStack {get; private set;}

        public StateMachine(T owner){
            this.owner = owner;
            StateStack = new Stack<State<T>>();
        }

        public void Push(State<T> state){
            StateStack.Push(state);
            CurrentState = state;
            CurrentState.Enter(owner);
        }

        public void Pop(){
            StateStack.Pop();
            CurrentState.Exit();
            CurrentState = StateStack.Peek();
        }

        public void Execute(){
            CurrentState?.Execute();
        }

        public void ChangeState(State<T> state){
            if (CurrentState != null){
                StateStack.Pop();
                CurrentState.Exit();
            }
            StateStack.Push(state);
            CurrentState = state;
            CurrentState.Enter(owner);
        }
    }
}