using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 상태를 객체화 한다. IDLE 상태에선 Run이 될 수도 있고 Hit
 * 
 */
public abstract class State<T>
{
    protected State<T> _state;

    public delegate void StateFunc(T t);
    public StateFunc _stateFunc;

    public virtual void Enter(T t)  { _stateFunc = Update; }
    public virtual void Update(T t) {}
    public virtual void Exit(T t)   {}
    public State() 
    { 
        _stateFunc = Enter;
        _state = this;
    }
}