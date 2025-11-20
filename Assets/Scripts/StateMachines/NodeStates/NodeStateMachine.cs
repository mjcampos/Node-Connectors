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
    public NodeGraphController NodeGraphController { get; private set; }

    void Awake()
    {
        InitializeComponents();
    }

    void OnValidate()
    {

        InitializeComponents();
        UpdateStateFromEnum();
    }

    void InitializeComponents()
    {
        if (NodeGraphController == null)
            NodeGraphController = GetComponentInParent<NodeGraphController>();
        
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();
        
        if (AdjacentNodes == null)
            AdjacentNodes = GetComponent<AdjacentNodes>();
    }
    
    public void UpdateStateFromEnum()
    {
        if (SpriteRenderer == null) return;
        
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
