using System;
using UnityEngine;
using Helpers;

public class NodeStateMachine : StateMachine
{
    [Header("Node Data")]
    [SerializeField] NodeDataSO nodeData;
    
#if UNITY_EDITOR
    [SerializeField, HideInInspector] NodeDataSO previousNodeData;
#endif
    
    [Header("State References")]
    public NodeState state;
    public bool canBeUnlocked;
    public int degreesFromUnlocked;
    public int degreesFromVisible;
    public int degreesFromNonHoverable;
    
    [Header("Component References")]
    public Node Node { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public NodeGraphController NodeGraphController { get; private set; }
    public UIController UIController { get; private set; }

    public static event Action OnNodeStateChanged;

    void Awake()
    {
        InitializeComponents();
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            InitializeRuntimeState();
        }
    }

    void OnValidate()
    {
        InitializeComponents();
        
        if (nodeData == null)
        {
            Debug.LogWarning($"NodeStateMachine on '{gameObject.name}' has no NodeDataSO assigned!", this);
        }
        
#if UNITY_EDITOR
        if (previousNodeData != nodeData)
        {
            if (Node != null)
            {
                Node.OnNodeDataChanged();
            }
            
            previousNodeData = nodeData;
        }
#endif
        
        UpdateStateFromEnum();
        
        if (!Application.isPlaying)
        {
            RippleAllUnlockedNodesInGraph();
        }
    }

    void InitializeComponents()
    {
        if (Node == null)
            Node = GetComponent<Node>();
        
        if (NodeGraphController == null)
            NodeGraphController = GetComponentInParent<NodeGraphController>();
        
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();
        
        if (UIController == null)
            UIController = GetComponent<UIController>();
    }

    void InitializeRuntimeState()
    {
        RippleAllUnlockedNodesInGraph();
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

    void RippleAllUnlockedNodesInGraph()
    {
        if (NodeGraphController == null) return;
        
        NodeStateMachine[] allStateMachines = NodeGraphController.GetComponentsInChildren<NodeStateMachine>();
        
        bool hasUnlockedNodes = false;
        foreach (NodeStateMachine stateMachine in allStateMachines)
        {
            if (stateMachine.state == NodeState.Unlocked)
            {
                hasUnlockedNodes = true;
                break;
            }
        }
        
        if (!hasUnlockedNodes)
        {
            UpdateDegreesText();
            return;
        }
        
        foreach (NodeStateMachine stateMachine in allStateMachines)
        {
            if (stateMachine.state == NodeState.Visible || 
                stateMachine.state == NodeState.NonHoverable || 
                stateMachine.state == NodeState.Hidden)
            {
                stateMachine.degreesFromUnlocked = int.MaxValue;
                stateMachine.canBeUnlocked = false;
            }
        }
        
        foreach (NodeStateMachine stateMachine in allStateMachines)
        {
            if (stateMachine.state == NodeState.Unlocked)
            {
                stateMachine.Ripple();
            }
        }
        
        foreach (NodeStateMachine stateMachine in allStateMachines)
        {
            stateMachine.UpdateDegreesText();
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
            UpdateStateFromEnum();
            
            RippleAllUnlockedNodesInGraph();
            
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
        
        if (displayValue == int.MaxValue)
        {
            UIController.SetDegreesText(string.Empty);
        }
        else
        {
            UIController.SetDegreesText(displayValue.ToString());
        }
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

    public int GetHoverableRange()
    {
        return NodeGraphController != null ? NodeGraphController.GetHoverableRange() : int.MaxValue;
    }

    public int GetNonHoverableRange()
    {
        return NodeGraphController != null ? NodeGraphController.GetNonHoverableRange() : int.MaxValue;
    }

    public void SetSprite(NodeState newState)
    {
        Sprite sprite = nodeData?.GetSpriteForState(newState);
        
        if (SpriteRenderer != null)
        {
            SpriteRenderer.sprite = sprite;
        }
    }

    public NodeDataSO GetNodeData()
    {
        return nodeData;
    }

    public void HoverEnter()
    {
        OnHoverEnter();
    }

    public void HoverExit()
    {
        OnHoverExit();
    }
}
