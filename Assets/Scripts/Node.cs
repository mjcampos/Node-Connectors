using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

[ExecuteAlways]
public class Node : MonoBehaviour
{
    public Node[] adjacentNodes;
    
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
