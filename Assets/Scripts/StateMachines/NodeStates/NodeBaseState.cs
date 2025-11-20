using Helpers;
using UnityEngine;

public abstract class NodeBaseState : State
{
    protected NodeStateMachine StateMachine;

    public NodeBaseState(NodeStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    protected void CalculateDegrees()
    {
        switch (StateMachine.previousState)
        {
            case NodeState.Locked:
                StateMachine.degreesFromUnlocked = 0;
                break;
            case NodeState.Unlocked:
                StateMachine.degreesFromUnlocked = 1;
                break;
            case NodeState.Visible:
            case NodeState.NonHoverable:
            case NodeState.Hidden:
                StateMachine.degreesFromUnlocked = StateMachine.previousDegrees + 1;
                break;
            default:
                StateMachine.degreesFromUnlocked = 0;
                break;
        }
    }

    public void TraverseNeighbors(bool canBeUnlocked)
    {
        StateMachine.UpdateDegreesText();
        
        if (StateMachine.AdjacentNodes == null) return;
        
        int hoverableRange = StateMachine.GetHoverableRange();
        int nonHoverableRange = StateMachine.GetNonHoverableRange();
        
        foreach (var neighbor in StateMachine.AdjacentNodes.neighborNodes)
        {
            if (neighbor == null) continue;
            
            NodeStateMachine neighborStateMachine = neighbor.GetComponent<NodeStateMachine>();

            if (neighborStateMachine == null) continue;

            int newDegreesFromUnlocked = StateMachine.degreesFromUnlocked + 1;
            
            neighborStateMachine.previousState = StateMachine.state;
            
            if (neighborStateMachine.state == NodeState.Visible || 
                neighborStateMachine.state == NodeState.NonHoverable ||
                neighborStateMachine.state == NodeState.Hidden)
            {
                bool shouldUpdate = newDegreesFromUnlocked < neighborStateMachine.degreesFromUnlocked ||
                                    neighborStateMachine.degreesFromUnlocked == 0;

                if (shouldUpdate)
                {
                    neighborStateMachine.previousDegrees = StateMachine.degreesFromUnlocked;
                    neighborStateMachine.degreesFromUnlocked = newDegreesFromUnlocked;
                    neighborStateMachine.canBeUnlocked = canBeUnlocked && (newDegreesFromUnlocked <= hoverableRange);

                    NodeState targetState;

                    if (newDegreesFromUnlocked <= hoverableRange)
                    {
                        targetState = NodeState.Visible;
                    }
                    else
                    {
                        int degreesFromVisible = newDegreesFromUnlocked - hoverableRange;

                        neighborStateMachine.degreesFromVisible = degreesFromVisible;

                        if (degreesFromVisible <= nonHoverableRange)
                        {
                            targetState = NodeState.NonHoverable;
                        }
                        else
                        {
                            targetState = NodeState.Hidden;
                            int degreesFromNonHoverable = degreesFromVisible - nonHoverableRange;
                            neighborStateMachine.degreesFromNonHoverable = degreesFromNonHoverable;
                        }
                    }

                    if (neighborStateMachine.state != targetState)
                    {
                        neighborStateMachine.state = targetState;
                        neighborStateMachine.UpdateStateFromEnum();
                    }
                }
            }
            else if (neighborStateMachine.state == NodeState.Unlocked)
            {
                neighborStateMachine.canBeUnlocked = canBeUnlocked;
                neighborStateMachine.previousDegrees = StateMachine.degreesFromUnlocked;
                neighborStateMachine.degreesFromUnlocked = 0;
            }
            else
            {
                neighborStateMachine.canBeUnlocked = !canBeUnlocked;
            }
            
            neighborStateMachine.OnRipple();
        }
        
        // Refresh edge visibility for this node after all neighbors have updated
        StateMachine.RefreshEdgeVisibility();
        
        // Also refresh edge visiblity for all neighbors
        foreach (var neighbor in StateMachine.AdjacentNodes.neighborNodes)
        {
            if (neighbor == null) continue;
            
            NodeStateMachine neighborStateMachine = neighbor.GetComponent<NodeStateMachine>();

            if (neighborStateMachine != null)
            {
                neighborStateMachine.RefreshEdgeVisibility();
            }
        }
    }
}
