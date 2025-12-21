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
        if (StateMachine.node == null) return;
        
        int hoverableRange = StateMachine.GetHoverableRange();
        int nonHoverableRange = StateMachine.GetNonHoverableRange();
        
        foreach (var neighbor in StateMachine.node.neighborNodes)
        {
            if (neighbor == null) continue;
            
            NodeStateMachine neighborStateMachine = neighbor.GetComponent<NodeStateMachine>();

            if (neighborStateMachine == null) continue;

            int newDegreesFromUnlocked = StateMachine.degreesFromUnlocked + 1;
            
            if (neighborStateMachine.state == NodeState.Locked)
            {
                bool shouldUpdate = newDegreesFromUnlocked < neighborStateMachine.degreesFromUnlocked;

                if (shouldUpdate)
                {
                    neighborStateMachine.degreesFromUnlocked = newDegreesFromUnlocked;
                    
                    int degreesFromVisible = newDegreesFromUnlocked - hoverableRange;
                    neighborStateMachine.degreesFromVisible = degreesFromVisible;

                    if (nonHoverableRange > 0)
                    {
                        if (degreesFromVisible <= nonHoverableRange)
                        {
                            neighborStateMachine.degreesFromNonHoverable = 0;
                        }
                        else
                        {
                            int degreesFromNonHoverable = degreesFromVisible - nonHoverableRange;
                            neighborStateMachine.degreesFromNonHoverable = degreesFromNonHoverable;
                        }
                    }
                    else
                    {
                        neighborStateMachine.degreesFromNonHoverable = degreesFromVisible;
                    }

                    neighborStateMachine.UpdateDegreesText();
                    neighborStateMachine.OnRipple();
                }
                
                continue;
            }
            
            if (neighborStateMachine.state == NodeState.Visible || 
                neighborStateMachine.state == NodeState.NonHoverable ||
                neighborStateMachine.state == NodeState.Hidden)
            {
                bool shouldUpdate = newDegreesFromUnlocked < neighborStateMachine.degreesFromUnlocked;

                if (shouldUpdate)
                {
                    neighborStateMachine.degreesFromUnlocked = newDegreesFromUnlocked;
                    
                    bool isClickable = canBeUnlocked && (newDegreesFromUnlocked <= hoverableRange);
                    neighborStateMachine.canBeUnlocked = isClickable;

                    NodeState targetState;

                    if (newDegreesFromUnlocked <= hoverableRange)
                    {
                        targetState = NodeState.Visible;
                        neighborStateMachine.degreesFromVisible = 0;
                        neighborStateMachine.degreesFromNonHoverable = 0;
                    }
                    else
                    {
                        int degreesFromVisible = newDegreesFromUnlocked - hoverableRange;
                        neighborStateMachine.degreesFromVisible = degreesFromVisible;

                        if (nonHoverableRange > 0 && degreesFromVisible <= nonHoverableRange)
                        {
                            targetState = NodeState.NonHoverable;
                            neighborStateMachine.degreesFromNonHoverable = 0;
                        }
                        else
                        {
                            targetState = NodeState.Hidden;
                            
                            if (nonHoverableRange > 0)
                            {
                                int degreesFromNonHoverable = degreesFromVisible - nonHoverableRange;
                                neighborStateMachine.degreesFromNonHoverable = degreesFromNonHoverable;
                            }
                            else
                            {
                                neighborStateMachine.degreesFromNonHoverable = degreesFromVisible;
                            }
                        }
                    }

                    if (neighborStateMachine.state != targetState)
                    {
                        neighborStateMachine.state = targetState;
                        neighborStateMachine.UpdateStateFromEnum();
                    }
                    
                    neighborStateMachine.UpdateDegreesText();
                    neighborStateMachine.OnRipple();
                }
            }
            else if (neighborStateMachine.state == NodeState.Unlocked)
            {
                neighborStateMachine.canBeUnlocked = canBeUnlocked;
            }
        }
    }
}
