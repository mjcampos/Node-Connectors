using System;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    State _currentState;

    public void SwitchState(State newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }
    
    void Update()
    {
        _currentState?.Tick(Time.deltaTime);
    }

    public void OnRipple()
    {
        _currentState?.RippleHandle();
    }

    public void OnHoverEnter()
    {
        _currentState?.HoverEnterHandle();
    }

    public void OnHoverExit()
    {
        _currentState?.HoverExitHandle();
    }
}
