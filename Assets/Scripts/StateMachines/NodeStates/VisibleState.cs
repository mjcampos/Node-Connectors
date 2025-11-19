using Helpers;
using UnityEngine;

public class VisibleState : NodeBaseState
{
    public VisibleState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.SpriteRenderer.color = NodeStateColors.Visible;
        StateMachine.canBeUnlocked = StateMachine.previousState == NodeState.Unlocked;

        switch (StateMachine.previousState)
        {
            case NodeState.Locked:
                StateMachine.degreesOfSeparationFromUnlocked = 0;
                break;
            case NodeState.Unlocked:
                StateMachine.degreesOfSeparationFromUnlocked = 1;
                break;
            case NodeState.Visible:
                StateMachine.degreesOfSeparationFromUnlocked = StateMachine.previousStateDegrees + 1;
                break;
            default:
                StateMachine.degreesOfSeparationFromUnlocked = 0;
                break;
        }
        
        StateMachine.UpdateDegreesText();
        StateMachine.Ripple();
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
    }

    public override void RippleHandle()
    {
        TraverseNeighbors(false);
    }
}
