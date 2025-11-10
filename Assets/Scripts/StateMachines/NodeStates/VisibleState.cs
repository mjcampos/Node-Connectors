using Helpers;
using UnityEngine;

public class VisibleState : NodeBaseState
{
    public VisibleState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.SpriteRenderer.color = NodeStateColors.Visible;
        
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
