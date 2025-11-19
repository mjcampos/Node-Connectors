using System;
using UnityEngine;
using Helpers;
using TMPro;

public class NodeStateMachine : StateMachine
{
    public NodeState state;
    public NodeState previousState = NodeState.None;
    public bool canBeUnlocked;
    public int degreesOfSeparationFromUnlocked;
    public int previousStateDegrees;

    public TextMeshProUGUI degreesOfSeparationText;
    
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
            case NodeState.Locked:
                SwitchState(new LockedState(this));
                break;
            case NodeState.Visible:
                SwitchState(new VisibleState(this));
                break;
            default:
                SwitchState(new UnlockedState(this));
                break;
        }
    }

    public void Ripple()
    {
        OnRipple();
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

    public void UpdateDegreesText()
    {
        degreesOfSeparationText.text = degreesOfSeparationFromUnlocked.ToString();
    }
}
