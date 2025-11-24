using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

[Serializable]
public class NodeData
{
    public string nodeName;
    public NodeState state;
    public bool canBeUnlocked;
    public int degreesFromUnlocked;
    public int degreesFromVisible;
    public int degreesFromNonHoverable;
    public string nodeText;
}

[Serializable]
public class NodeSaveData
{
    public List<NodeData> nodes = new List<NodeData>();

    public void Clear()
    {
        nodes.Clear();
    }

    public NodeData FindNodeData(string nodeName)
    {
        return nodes.Find(n => n.nodeName == nodeName);
    }

    public void AddOrUpdateNode(NodeData nodeData)
    {
        NodeData existing = FindNodeData(nodeData.nodeName);

        if (existing != null)
        {
            existing.state = nodeData.state;
            existing.canBeUnlocked = nodeData.canBeUnlocked;
            existing.degreesFromUnlocked = nodeData.degreesFromUnlocked;
            existing.degreesFromVisible = nodeData.degreesFromVisible;
            existing.degreesFromNonHoverable = nodeData.degreesFromNonHoverable;
            existing.nodeText = nodeData.nodeText;
        }
        else
        {
            nodes.Add(nodeData);
        }
    }
    
    public List<NodeData> GetUnlockedNodes()
    {
        return nodes.FindAll(n => n.state == NodeState.Unlocked && !string.IsNullOrEmpty(n.nodeText));
    }
}
