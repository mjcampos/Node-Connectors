using Helpers;
using UnityEngine;

public class HiddenState : NodeBaseState
{
    public HiddenState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.SpriteRenderer.color = NodeStateColors.Hidden;
        StateMachine.canBeUnlocked = false;
        StateMachine.SetVisibility(false);
        StateMachine.UpdateDegreesText();
    }

    public override void Tick(float deltaTime)
    {
        
    }

    public override void Exit()
    {
        StateMachine.SetVisibility(true);
    }

    public override void RippleHandle()
    {
        TraverseNeighbors(false);
    }
}
