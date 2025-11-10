using System;
using UnityEngine;
using Helpers;

public class NodeStateMachine : StateMachine
{
    public NodeState state;
    
    public SpriteRenderer SpriteRenderer { get; private set; }
    public AdjacentNodes AdjacentNodes { get; private set; }

    void OnValidate()
    {
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();
        
        if (AdjacentNodes == null)
            AdjacentNodes = GetComponent<AdjacentNodes>();
        
        UpdateState();
    }
    
    void UpdateState()
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
            case NodeState.Default:
                SwitchState(new DefaultState(this));
                break;
            default:
                SwitchState(new DefaultState(this));
                break;
        }
    }
}
