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
        
        if (StateMachine.AdjacentNodes != null)
        {
            foreach (var neighbor in  StateMachine.AdjacentNodes.neighborNodes)
            {
                NodeStateMachine neighborStateMachine = neighbor.GetComponent<NodeStateMachine>();

                if (neighborStateMachine != null && neighborStateMachine.state == NodeState.Visible)
                {
                    neighborStateMachine.canBeUnlocked = true;
                }
                else
                {
                    neighborStateMachine.canBeUnlocked = false;
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
