using UnityEngine;
using Helpers;

public class NodeHoverHandler : MonoBehaviour
{
    NodeStateMachine _stateMachine;
    UIController _uiController;
    bool _isHovering;

    void Awake()
    {
        _stateMachine = GetComponent<NodeStateMachine>();
        _uiController = GetComponent<UIController>();
    }

    void OnMouseEnter()
    {
        if (!CanShowHoverText()) return;

        _isHovering = true;
        _uiController?.SetHoverTextVisibility(true);
    }

    void OnMouseExit()
    {
        if (!_isHovering) return;

        _isHovering = false;
        _uiController?.SetHoverTextVisibility(false);
    }

    bool CanShowHoverText()
    {
        if (_stateMachine == null || _uiController == null)
            return false;

        if (_stateMachine.state != NodeState.Visible)
            return false;

        if (!_uiController.HasHoverText())
            return false;

        return true;
    }

    void OnDisable()
    {
        if (_isHovering)
        {
            _isHovering = false;
            _uiController?.SetHoverTextVisibility(false);
        }
    }
}
