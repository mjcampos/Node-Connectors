using Helpers;
using UnityEngine;

public class NonHoverableState : NodeBaseState
{
    public NonHoverableState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.SpriteRenderer.color = NodeStateColors.NonHoverable;
        StateMachine.canBeUnlocked = false;
        StateMachine.SetVisibility(true);
        StateMachine.UpdateDegreesText();
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
