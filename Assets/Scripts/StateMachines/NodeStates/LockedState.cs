using Helpers;
using UnityEngine;

public class LockedState : NodeBaseState
{
    public LockedState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.spriteRenderer.color = NodeStateColors.Locked;
    }

    public override void Tick(float deltaTime)
    {
        
    }

    public override void Exit()
    {
        
    }
}
