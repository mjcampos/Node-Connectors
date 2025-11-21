using System;
using System.Collections.Generic;
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
    
    public SpriteRenderer SpriteRenderer { get; private set; }
    public AdjacentNodes AdjacentNodes { get; private set; }
    public NodeGraphController NodeGraphController { get; private set; }
    public UIController UIController { get; private set; }
    
    EdgeVisibilityManager _edgeVisibilityManager;

    public static event Action OnNodeStateChanged;

    void Awake()
    {
        InitializeComponents();
        InitializeEdgeManager();
    }

    void OnValidate()
    {

        InitializeComponents();
        InitializeEdgeManager();
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

        if (UIController == null)
            UIController = GetComponent<UIController>();
    }

    void InitializeEdgeManager()
    {
        if (_edgeVisibilityManager == null)
            _edgeVisibilityManager = new EdgeVisibilityManager(transform, this);
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
            
            NotifyStateChanged();
        }
    }

    void NotifyStateChanged()
    {
        OnNodeStateChanged?.Invoke();
    }

    public void UpdateDegreesText()
    {
        if (UIController == null) return;

        int displayValue = state switch
        {
            NodeState.NonHoverable => degreesFromVisible,
            NodeState.Hidden => degreesFromNonHoverable,
            _ => degreesFromUnlocked
        };
        
        UIController.SetDegreesText(displayValue.ToString());
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

        if (UIController != null)
            UIController.SetDegreesTextVisibility(isVisible);
    }

    public void RefreshEdgeVisibility()
    {
        _edgeVisibilityManager?.UpdateEdgeVisibility();
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
