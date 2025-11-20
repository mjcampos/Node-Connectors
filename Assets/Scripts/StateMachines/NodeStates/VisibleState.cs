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

        CalculateDegrees();

        StateMachine.degreesFromVisible = 0;
        StateMachine.SetVisibility(true);
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
