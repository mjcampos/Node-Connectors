using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class AdjacentNodes : MonoBehaviour
{
    public AdjacentNodes[] neighborNodes;
    
    public void AdjustEdges()
    {
        List<LineRenderer> connectionLines = new List<LineRenderer>();
        
        // Check if adjacent nodes exist. If not then do nothing.
        if (neighborNodes == null) return;
        
        // Destroy existing line children
        DestroyConnectionLines();
        
        // Create new line children
        foreach (var node in neighborNodes)
        {
            if (node == null) continue;
            
            var lineObject = new GameObject($"Edge_To_{node.name}");
            
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
            if (neighborNodes[i] == null) continue;
            
            var lr = connectionLines[i];
            
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, neighborNodes[i].transform.position);
        }
    }

    void DestroyConnectionLines()
    {
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "DegreesCanvas") continue;
            
            children.Add(transform.GetChild(i));
        }

        foreach (Transform child in children)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
