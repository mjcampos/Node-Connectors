using Helpers;
using UnityEngine;

public class VisibleState : NodeBaseState
{
    public VisibleState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.spriteRenderer.color = NodeStateColors.Visible;
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
    }
}
