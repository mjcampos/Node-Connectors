using Helpers;
using NodeController;

namespace NodeSystem.StateMachines.NodeStates
{
    public class NonHoverableState : NodeBaseState
    {
        public NonHoverableState(NodeStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            StateMachine.SetSprite(NodeState.NonHoverable);
            StateMachine.canBeUnlocked = false;
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
            TraverseNeighbors(false);
        }

        public override void HoverEnterHandle()
        {
            if (SoundPlayer.Instance != null)
            {
                SoundPlayer.Instance.PlayHoverSound();
            }
        }
    }
}
