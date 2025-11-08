using Helpers;
using UnityEngine;

public class UnlockedState : NodeBaseState
{
    public UnlockedState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.spriteRenderer.color = NodeStateColors.Unlocked;
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
    }
}
