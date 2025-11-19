using Helpers;
using UnityEngine;

public abstract class NodeBaseState : State
{
    protected NodeStateMachine StateMachine;

    public NodeBaseState(NodeStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public void TravereNeighbors(bool canBeUnlocked)
    {
        StateMachine.UpdateDegreesText();
        
        if (StateMachine.AdjacentNodes != null)
        {
            foreach (var neighbor in  StateMachine.AdjacentNodes.neighborNodes)
            {
                NodeStateMachine neighborStateMachine = neighbor.GetComponent<NodeStateMachine>();

                if (neighborStateMachine != null)
                {
                    if (neighborStateMachine.state == NodeState.Visible)
                    {
                        neighborStateMachine.canBeUnlocked = canBeUnlocked;
                        neighborStateMachine.previousStateDegrees = StateMachine.degreesOfSeparationFromUnlocked;
                        neighborStateMachine.degreesOfSeparationFromUnlocked = StateMachine.degreesOfSeparationFromUnlocked + 1;
                    }
                    else
                    {
                        neighborStateMachine.canBeUnlocked = !canBeUnlocked;
                    }
                }
                
                neighborStateMachine.previousState = StateMachine.state;
                
                neighborStateMachine.OnRipple();
            }
        }
    }
}
