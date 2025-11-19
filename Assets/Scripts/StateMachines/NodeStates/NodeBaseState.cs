using Helpers;
using UnityEngine;

public abstract class NodeBaseState : State
{
    protected NodeStateMachine StateMachine;

    public NodeBaseState(NodeStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    protected void CalculateDegreesOfSeparation()
    {
        switch (StateMachine.previousState)
        {
            case NodeState.Locked:
                StateMachine.degreesOfSeparationFromUnlocked = 0;
                break;
            case NodeState.Unlocked:
                StateMachine.degreesOfSeparationFromUnlocked = 1;
                break;
            case NodeState.Visible:
            case NodeState.NonHoverable:
                StateMachine.degreesOfSeparationFromUnlocked = StateMachine.previousStateDegrees + 1;
                break;
            default:
                StateMachine.degreesOfSeparationFromUnlocked = 0;
                break;
        }
    }

    public void TraverseNeighbors(bool canBeUnlocked)
    {
        StateMachine.UpdateDegreesText();
        
        if (StateMachine.AdjacentNodes == null) return;
        
        int visibilityThreshold = StateMachine.GetVisibilityThreshold();
        
        foreach (var neighbor in StateMachine.AdjacentNodes.neighborNodes)
        {
            if (neighbor == null) continue;
            
            NodeStateMachine neighborStateMachine = neighbor.GetComponent<NodeStateMachine>();

            if (neighborStateMachine == null) continue;

            int newDegrees = StateMachine.degreesOfSeparationFromUnlocked + 1;
            
            neighborStateMachine.previousState = StateMachine.state;
            
            if (neighborStateMachine.state == NodeState.Visible || neighborStateMachine.state == NodeState.NonHoverable)
            {
                neighborStateMachine.previousStateDegrees = StateMachine.degreesOfSeparationFromUnlocked;
                neighborStateMachine.degreesOfSeparationFromUnlocked = newDegrees;
                neighborStateMachine.canBeUnlocked = canBeUnlocked && (newDegrees <= visibilityThreshold);
                
                // Determine if should be visible or NonHoverable based on degrees
                NodeState targetState = newDegrees <= visibilityThreshold
                    ? NodeState.Visible
                    : NodeState.NonHoverable;

                if (neighborStateMachine.state != targetState)
                {
                    neighborStateMachine.state = targetState;
                    neighborStateMachine.UpdateStateFromEnum();
                }
            }
            else if (neighborStateMachine.state == NodeState.Unlocked)
            {
                neighborStateMachine.canBeUnlocked = canBeUnlocked;
                neighborStateMachine.previousStateDegrees = StateMachine.degreesOfSeparationFromUnlocked;
                neighborStateMachine.degreesOfSeparationFromUnlocked = 0;
            }
            else
            {
                neighborStateMachine.canBeUnlocked = !canBeUnlocked;
            }
            
            neighborStateMachine.OnRipple();
        }
    }
}
