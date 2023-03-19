using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 상태를 객체화 한다. IDLE 상태에선 Run이 될 수도 있고 Hit
 * 
 */
public interface IState
{
    protected void OnEnter();
    protected void OnUpdate();
    protected void OnExit();
}

public class IdleState : IState
{
    void IState.OnEnter() {}
    void IState.OnUpdate() {}
    void IState.OnExit() {}
}