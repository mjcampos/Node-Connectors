using System;
using UnityEngine;
using Helpers;

public class NodeStateMachine : StateMachine
{
    public NodeState state;
    
    public SpriteRenderer spriteRenderer;

    void OnValidate()
    {
        UpdateColor();
    }
    
    void UpdateColor()
    {
        switch (state)
        {
            case NodeState.Locked:
                SwitchState(new LockedState(this));
                break;
            case NodeState.Unlocked:
                SwitchState(new UnlockedState(this));
                break;
            case NodeState.Visible:
                SwitchState(new VisibleState(this));
                break;
            case NodeState.NonHoverable:
                SwitchState(new NonhoverableState(this));
                break;
            case NodeState.Hidden:
                SwitchState(new HiddenState(this));
                break;
            default:
                SwitchState(new DefaultState(this));
                break;
        }
    }
}
