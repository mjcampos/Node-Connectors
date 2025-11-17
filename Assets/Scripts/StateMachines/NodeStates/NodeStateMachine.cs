using System;
using UnityEngine;
using Helpers;

public class NodeStateMachine : StateMachine
{
    public NodeState state;
    
    public SpriteRenderer SpriteRenderer { get; private set; }

    void OnValidate()
    {
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();
        
        UpdateState();
    }
    
    void UpdateState()
    {
        switch (state)
        {
            case NodeState.Unlocked:
                SwitchState(new UnlockedState(this));
                break;
            case NodeState.Visible:
                SwitchState(new VisibleState(this));
                break;
            default:
                SwitchState(new UnlockedState(this));
                break;
        }
    }
}
