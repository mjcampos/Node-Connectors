using Helpers;
using UnityEngine;

public class UnlockedState : NodeBaseState
{
    public UnlockedState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.SetSprite(NodeState.Unlocked);
        StateMachine.degreesFromUnlocked = 0;
        
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
        TraverseNeighbors(true);
    }

    public override void HoverEnterHandle()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHoverSound();
        }
    }
}
