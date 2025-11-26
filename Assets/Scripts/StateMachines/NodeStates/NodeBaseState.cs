using Helpers;
using UnityEngine;

public abstract class NodeBaseState : State
{
    protected NodeStateMachine StateMachine;

    public NodeBaseState(NodeStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public void TraverseNeighbors(bool canBeUnlocked)
    {
        StateMachine.UpdateDegreesText();
        
        if (StateMachine.Node == null) return;
        
        int hoverableRange = StateMachine.GetHoverableRange();
        int nonHoverableRange = StateMachine.GetNonHoverableRange();
        
        foreach (var neighbor in StateMachine.Node.neighborNodes)
        {
            if (neighbor == null) continue;
            
            NodeStateMachine neighborStateMachine = neighbor.GetComponent<NodeStateMachine>();

            if (neighborStateMachine == null) continue;

            int newDegreesFromUnlocked = StateMachine.degreesFromUnlocked + 1;
            
            if (neighborStateMachine.state == NodeState.Visible || 
                neighborStateMachine.state == NodeState.NonHoverable ||
                neighborStateMachine.state == NodeState.Hidden)
            {
                bool shouldUpdate = newDegreesFromUnlocked < neighborStateMachine.degreesFromUnlocked ||
                                    neighborStateMachine.degreesFromUnlocked == 0;

                if (shouldUpdate)
                {
                    neighborStateMachine.degreesFromUnlocked = newDegreesFromUnlocked;
                    neighborStateMachine.UpdateDegreesText();
                    
                    bool isClickable = canBeUnlocked && (newDegreesFromUnlocked <= hoverableRange);
                    
                    neighborStateMachine.canBeUnlocked = neighborStateMachine.canBeUnlocked || isClickable;

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
                    else
                    {
                        neighborStateMachine.OnRipple();
                    }
                }
                else
                {
                    bool isClickable = canBeUnlocked && (newDegreesFromUnlocked <= hoverableRange);
                    
                    neighborStateMachine.canBeUnlocked = neighborStateMachine.canBeUnlocked || isClickable;
                }
            }
            else if (neighborStateMachine.state == NodeState.Unlocked)
            {
                neighborStateMachine.canBeUnlocked = canBeUnlocked;
                neighborStateMachine.degreesFromUnlocked = 0;
                neighborStateMachine.UpdateDegreesText();
            }
            else
            {
                neighborStateMachine.canBeUnlocked = !canBeUnlocked;
            }
        }
    }
}
