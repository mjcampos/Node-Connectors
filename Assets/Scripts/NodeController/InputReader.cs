using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    Controls _controls;
    Camera _camera;
    NodeStateMachine _currentHoveredNode;
    
    void Start()
    {
        _controls = new Controls();
        _camera = Camera.main;
        
        _controls.Player.SetCallbacks(this);
        _controls.Player.Enable();
    }

    void Update()
    {
        UpdateHover();
    }

    void OnDestroy()
    {
        if (_controls != null)
        {
            _controls.Player.Disable();
            _controls.Dispose();
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (_camera == null)
        {
            _camera = Camera.main;
            
            if (_camera == null) return;
        }
        
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = _camera.ScreenPointToRay(screenPos);

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            NodeStateMachine clickedSM = hit.collider.GetComponent<NodeStateMachine>();
            
            clickedSM?.OnClick();
        }
    }

    void UpdateHover()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null) return;
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = _camera.ScreenPointToRay(screenPos);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        NodeStateMachine hoveredNode = null;

        if (hit.collider != null)
        {
            hoveredNode = hit.collider.GetComponent<NodeStateMachine>();
        }

        if (hoveredNode != _currentHoveredNode)
        {
            if (_currentHoveredNode != null)
            {
                _currentHoveredNode.HoverExit();
            }

            _currentHoveredNode = hoveredNode;

            if (_currentHoveredNode != null)
            {
                _currentHoveredNode.HoverEnter();
            }
        }
    }
}
