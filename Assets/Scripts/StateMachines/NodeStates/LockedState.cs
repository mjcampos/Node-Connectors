using Helpers;
using UnityEngine;

public class LockedState : NodeBaseState
{
    public LockedState(NodeStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        StateMachine.SpriteRenderer.color = NodeStateColors.Locked;
        StateMachine.Ripple();
    }

    public override void Tick(float deltaTime)
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void RippleHandle()
    {
        StateMachine.canBeUnlocked = false;
        
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
                
                neighborStateMachine.OnRipple();
            }
        }
    }
}
