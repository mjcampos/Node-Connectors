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
        StateMachine.degreesOfSeparationFromUnlocked = (StateMachine.previousState == NodeState.Unlocked) ? 1 : StateMachine.previousStateDegrees + 1;
        
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
        TravereNeighbors(false);
    }
}
