using Helpers;
using UnityEngine;

public class LockedState : NodeBaseState
{
    public LockedState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.SpriteRenderer.color = NodeStateColors.Locked;
        StateMachine.degreesFromUnlocked = 0;
        
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
        StateMachine.canBeUnlocked = false;
    }
}
