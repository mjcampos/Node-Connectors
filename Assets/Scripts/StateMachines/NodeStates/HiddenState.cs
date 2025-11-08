using Helpers;
using UnityEngine;

public class HiddenState : NodeBaseState
{
    public HiddenState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.spriteRenderer.color = NodeStateColors.Hidden;
    }

    public override void Tick(float deltaTime)
    {
        
    }

    public override void Exit()
    {
        
    }
}
