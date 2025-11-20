using System;
using UnityEngine;
using Helpers;
using TMPro;

public class NodeStateMachine : StateMachine
{
    public NodeState state;
    public NodeState previousState = NodeState.None;
    public bool canBeUnlocked;
    
    public int degreesFromUnlocked;
    public int degreesFromVisible;
    public int degreesFromNonHoverable;
    
    public int previousDegrees;

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
            NodeState.Hidden => new HiddenState(this),
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
        if (degreesOfSeparationText == null) return;

        int displayValue = state switch
        {
            NodeState.NonHoverable => degreesFromVisible,
            NodeState.Hidden => degreesFromNonHoverable,
            _ => degreesFromUnlocked
        };
        
        degreesOfSeparationText.text = displayValue.ToString();
    }

    public void SetVisibility(bool isVisible)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            isVisible = true;
        }
#endif

        if (SpriteRenderer != null)
            SpriteRenderer.enabled = isVisible;

        if (degreesOfSeparationText != null)
            degreesOfSeparationText.enabled = isVisible;
    }

    public int GetHoverableRange()
    {
        return NodeGraphController != null ? NodeGraphController.GetHoverableRange() : int.MaxValue;
    }

    public int GetNonHoverableRange()
    {
        return NodeGraphController != null ? NodeGraphController.GetNonHoverableRange() : int.MaxValue;
    }
}
