using Helpers;
using UnityEngine;

public class VisibleState : NodeBaseState
{
    public VisibleState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.SetSprite(NodeState.Visible);
        StateMachine.SetVisibility(true);
        StateMachine.UpdateDegreesText();
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
        HoverExitHandle();
    }

    public override void RippleHandle()
    {
        TraverseNeighbors(false);
    }

    public override void HoverEnterHandle()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHoverSound();
        }
        
        NodeDataSO nodeData = StateMachine.GetNodeData();

        if (nodeData != null && StateMachine.uiController != null)
        {
            StateMachine.uiController.ShowHoverInfo(nodeData.title, nodeData.description);
        }
    }

    public override void HoverExitHandle()
    {
        if (StateMachine.uiController != null)
        {
            StateMachine.uiController.HideHoverInfo();
        }
    }
}
