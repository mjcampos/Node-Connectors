using System;
using UnityEngine;
using Helpers;
using TMPro;
using UnityEditor.Experimental.GraphView;

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
    public NodeGraphController NodeGraphController { get; private set; }

    void Awake()
    {
        NodeGraphController = GetComponentInParent<NodeGraphController>();
    }

    void OnValidate()
    {
        if (NodeGraphController == null)
            NodeGraphController = GetComponentInParent<NodeGraphController>();
        
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();
        
        if (AdjacentNodes == null)
            AdjacentNodes = GetComponent<AdjacentNodes>();
        
        UpdateStateFromEnum();
    }
    
    public void UpdateStateFromEnum()
    {
        State newState = state switch
        {
            NodeState.Unlocked => new UnlockedState(this),
            NodeState.Locked => new LockedState(this),
            NodeState.Visible => new VisibleState(this),
            NodeState.NonHoverable => new NonHoverableState(this),
            _ => new UnlockedState(this)
        };
        
        SwitchState(newState);
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
    }

    public void UpdateDegreesText()
    {
        if (degreesOfSeparationText != null)
        {
            degreesOfSeparationText.text = degreesOfSeparationFromUnlocked.ToString();
        }
    }

    public int GetVisibilityThreshold()
    {
        return NodeGraphController != null ? NodeGraphController.GetVisibilityDegreeThreshold() : int.MaxValue;
    }
}
