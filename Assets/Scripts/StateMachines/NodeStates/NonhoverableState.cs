using Helpers;
using UnityEngine;

public class NonhoverableState : NodeBaseState
{
    public NonhoverableState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.spriteRenderer.color = NodeStateColors.NonHoverable;
    }

    public override void Tick(float deltaTime)
    {
        
    }

    public override void Exit()
    {
        
    }
}
