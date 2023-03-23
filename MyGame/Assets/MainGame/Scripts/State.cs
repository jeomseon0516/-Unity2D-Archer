using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;

/*
 * 상태를 객체화 한다. 
 */

public interface IState<T> where T : ObjectBase
{
    public void Enter(T t);
    public void Update(T t);
    public void Exit(T t);
}
public abstract class State<T> : IState<T> where T : ObjectBase
{
    public delegate void StateFunc(T t);
    private StateFunc _stateFunc;

    public virtual void Enter(T t)  { _stateFunc = Update; }
    public virtual void Update(T t) {}
    public virtual void Exit(T t)   {}

    public StateFunc GetStateFunc() { return _stateFunc; }
    public State() { _stateFunc = Enter; }
}
public class StateMachine<T> where T : ObjectBase
{
    protected State<T> _state;
    protected State<T> _beforeState = null;
    public void Update(T t) 
    {
        // 상태가 변경되면 이전 상태의 Exit를 호출
        if (_state != _beforeState && _beforeState != null)
            _beforeState.Exit(t);

        _state.GetStateFunc()(t);
        _beforeState = _state;
    }
    public State<T> GetState() { return _state; }
    public void SetState(State<T> state) { _state = state; }
}