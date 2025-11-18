using System;
using UnityEngine;
using Helpers;

public class NodeStateMachine : StateMachine
{
    public NodeState state;
    public bool canBeUnlocked;
    
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

    public void OnClick()
    {
        if (state == NodeState.Visible && canBeUnlocked)
        {
            state = NodeState.Unlocked;
            canBeUnlocked = false;
            SwitchState(new UnlockedState(this));
        }
        else
        {
            Debug.Log("Keep current State");
        }
    }
}
