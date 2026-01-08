using UnityEngine;
using UnityEngine.EventSystems;

namespace NodeSystem
{
    public class NodeUIInteraction : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        NodeStateMachine _stateMachine;

        void Awake()
        {
            _stateMachine = GetComponent<NodeStateMachine>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_stateMachine != null)
            {
                _stateMachine.OnClick();
            }
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
