using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

[ExecuteAlways]
public class Node : MonoBehaviour
{
    public NodeState state;
    public Node[] adjacentNodes;
    
    SpriteRenderer _spriteRenderer;

    void OnValidate()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        
        UpdateColor();
    }

    void UpdateColor()
    {
        switch (state)
        {
            case NodeState.Locked:
                _spriteRenderer.color = NodeStateColors.Locked;
                break;
            case NodeState.Unlocked:
                _spriteRenderer.color = NodeStateColors.Unlocked;
                break;
            case NodeState.Visible:
                _spriteRenderer.color = NodeStateColors.Visible;
                break;
            case NodeState.NonHoverable:
                _spriteRenderer.color = NodeStateColors.NonHoverable;
                break;
            case NodeState.Hidden:
                _spriteRenderer.color = NodeStateColors.Hidden;
                break;
            default:
                _spriteRenderer.color = Color.white;
                break;
        }
    }

    public void AdjustEdges()
    {
        List<LineRenderer> connectionLines = new List<LineRenderer>();
        
        // Check if adjacent nodes exist. If not then do nothing.
        if (adjacentNodes == null) return;
        
        // Destroy existing line children
        DestroyConnectionLines();
        
        // Create new line children
        foreach (var node in adjacentNodes)
        {
            if (node == null) continue;
            
            var lineObject = new GameObject($"LineTo_{node.name}");
            
            lineObject.transform.SetParent(transform);
            
            var lr = lineObject.AddComponent<LineRenderer>();

            lr.startColor = Color.yellow;
            lr.endColor = Color.yellow;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.positionCount = 2;
            
            connectionLines.Add(lr);
        }
        
        for (int i = 0; i < connectionLines.Count; i++)
        {
            if (adjacentNodes[i] == null) continue;
            
            var lr = connectionLines[i];
            
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, adjacentNodes[i].transform.position);
        }
    }

    void DestroyConnectionLines()
    {
        Transform[] children = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        foreach (Transform child in children)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
