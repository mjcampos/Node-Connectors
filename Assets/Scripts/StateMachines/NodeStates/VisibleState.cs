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
        
        if (StateMachine.AdjacentNodes != null)
        {
            foreach (var neighbor in  StateMachine.AdjacentNodes.neighborNodes)
            {
                NodeStateMachine neighborStateMachine = neighbor.GetComponent<NodeStateMachine>();

                if (neighborStateMachine != null && neighborStateMachine.state == NodeState.Visible)
                {
                    neighborStateMachine.canBeUnlocked = false;
                }
                else
                {
                    neighborStateMachine.canBeUnlocked = true;
                }
            }
        }
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
    }
}
