using Helpers;
using UnityEngine;

public class EdgeVisibilityManager
{
    readonly Transform _nodeTransform;
    readonly NodeStateMachine _nodeStateMachine;

    public EdgeVisibilityManager(Transform transform, NodeStateMachine stateMachine)
    {
        _nodeTransform = transform;
        _nodeStateMachine = stateMachine;
    }

    public void UpdateEdgeVisibility()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        if (_nodeStateMachine.AdjacentNodes == null || _nodeStateMachine.AdjacentNodes.neighborNodes == null) return;

        bool isNodeVisible = _nodeStateMachine.state != NodeState.Hidden;

        foreach (Transform child in _nodeTransform)
        {
            if (!child.name.StartsWith("Edge_To_")) continue;
            
            LineRenderer edgeRenderer = child.GetComponent<LineRenderer>();
            
            if (edgeRenderer == null) continue;
            
            edgeRenderer.enabled = isNodeVisible && IsTargetNodeVisible(child.name);
        }
    }

    bool IsTargetNodeVisible(string edgeName)
    {
        foreach (var neighbor in _nodeStateMachine.AdjacentNodes.neighborNodes)
        {
            if (neighbor == null) continue;

            if (edgeName.Contains(neighbor.name))
            {
                NodeStateMachine neighborStateMachine = neighbor.GetComponent<NodeStateMachine>();
                
                return neighborStateMachine == null || neighborStateMachine.state != NodeState.Hidden;
            }
        }

        return true;
    }
}
