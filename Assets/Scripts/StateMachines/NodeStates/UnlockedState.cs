using Helpers;
using UnityEngine;

public class UnlockedState : NodeBaseState
{
    public UnlockedState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.SpriteRenderer.color = NodeStateColors.Unlocked;

        foreach (AdjacentNodes adjacentNode in StateMachine.AdjacentNodes.nodes)
        {
            NodeStateMachine adjacentStateMachine = adjacentNode.GetComponent<NodeStateMachine>();

            adjacentStateMachine.state = NodeState.Visible;
            adjacentStateMachine.SwitchState(new VisibleState(adjacentStateMachine));
        }
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
    }
}
