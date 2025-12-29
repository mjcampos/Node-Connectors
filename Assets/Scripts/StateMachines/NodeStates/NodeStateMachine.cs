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
    public Node node;
    public SpriteRenderer spriteRenderer;
    public UIController uiController;

    [HideInInspector]
    public NodeGraphController nodeGraphController;

    public string NodeID => node.NodeID;

    public static event Action OnNodeStateChanged;

    void Awake()
    {
        if (nodeGraphController == null)
        {
            nodeGraphController = GetComponentInParent<NodeGraphController>();
        }
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
        if (nodeGraphController == null)
        {
            nodeGraphController = GetComponentInParent<NodeGraphController>();
        }

        if (nodeData == null)
        {
            Debug.LogWarning($"NodeStateMachine on '{gameObject.name}' has no NodeDataSO assigned!", this);
        }
        
#if UNITY_EDITOR
        if (previousNodeData != nodeData)
        {
            if (node != null)
            {
                node.OnNodeDataChanged();
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

    void InitializeRuntimeState()
    {
        RippleAllUnlockedNodesInGraph();
    }
    
    public void UpdateStateFromEnum()
    {
        if (spriteRenderer == null) return;
        
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
        if (nodeGraphController == null) return;
    
        NodeStateMachine[] allStateMachines = nodeGraphController.GetComponentsInChildren<NodeStateMachine>();
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
                stateMachine.state == NodeState.Hidden ||
                stateMachine.state == NodeState.Locked)
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
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClickSound();
            }
            
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
        if (uiController == null) return;

        int displayValue = state switch
        {
            NodeState.NonHoverable => degreesFromVisible,
            NodeState.Hidden => degreesFromNonHoverable,
            _ => degreesFromUnlocked
        };
        
        if (displayValue == int.MaxValue)
        {
            uiController.SetDegreesText(string.Empty);
        }
        else
        {
            uiController.SetDegreesText(displayValue.ToString());
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

        if (spriteRenderer != null)
            spriteRenderer.enabled = isVisible;

        if (uiController != null)
            uiController.SetDegreesTextVisibility(isVisible);
    }

    public int GetHoverableRange()
    {
        return nodeGraphController != null ? nodeGraphController.GetHoverableRange() : int.MaxValue;
    }

    public int GetNonHoverableRange()
    {
        return nodeGraphController != null ? nodeGraphController.GetNonHoverableRange() : int.MaxValue;
    }

    public void SetSprite(NodeState newState)
    {
        Sprite sprite = nodeData?.GetSpriteForState(newState);
        
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
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
