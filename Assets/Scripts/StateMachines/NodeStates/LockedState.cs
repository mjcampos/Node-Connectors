using Helpers;
using UnityEngine;

public class LockedState : NodeBaseState
{
    public LockedState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.SetSprite(NodeState.Locked);
        StateMachine.canBeUnlocked = false;
        
        UpdateVisibility();
        
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
        UpdateVisibility();
        TraverseNeighbors(false);
    }

    public override void HoverEnterHandle()
    {
        if (AudioManager.Instance != null && StateMachine.spriteRenderer != null && StateMachine.spriteRenderer.enabled)
        {
            AudioManager.Instance.PlayHoverSound();
        }
    }

    void UpdateVisibility()
    {
        int hoverableRange = StateMachine.GetHoverableRange();
        int nonHoverableRange = StateMachine.GetNonHoverableRange();
        int totalVisibleRange = hoverableRange + nonHoverableRange;
        bool shouldBeVisible = StateMachine.degreesFromUnlocked <= totalVisibleRange;
        
        StateMachine.SetVisibility(shouldBeVisible);
    }
}
