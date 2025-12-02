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
        NodeDataSO nodeData = StateMachine.GetNodeData();

        if (nodeData != null && StateMachine.UIController != null)
        {
            StateMachine.UIController.ShowHoverInfo(nodeData.title, nodeData.description);
        }
    }

    public override void HoverExitHandle()
    {
        if (StateMachine.UIController != null)
        {
            StateMachine.UIController.HideHoverInfo();
        }
    }
}
