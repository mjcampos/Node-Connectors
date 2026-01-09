using UnityEngine;
using UnityEngine.EventSystems;

namespace NodeSystem
{
    public class NodeUIInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        NodeStateMachine _stateMachine;

        void Awake()
        {
            _stateMachine = GetComponent<NodeStateMachine>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_stateMachine != null)
            {
                _stateMachine.HoverEnter();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_stateMachine != null)
            {
                _stateMachine.HoverExit();
            }
        }
    }
}
