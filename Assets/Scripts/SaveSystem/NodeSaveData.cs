using System;
using System.Collections.Generic;
using Helpers;

[Serializable]
public class NodeSaveData
{
    public string nodeID;
    public NodeState state;
    public bool canBeUnlocked;
    public int degreesFromUnlocked;
    public int degreesFromVisible;
    public int degreesFromNonHoverable;

    public NodeSaveData(string id, NodeState nodeState, bool unlockable, int degUnlocked, int degVisible, int degNonHoverable)
    {
        nodeID = id;
        state = nodeState;
        canBeUnlocked = unlockable;
        degreesFromUnlocked = degUnlocked;
        degreesFromVisible = degVisible;
        degreesFromNonHoverable = degNonHoverable;
    }
}

[Serializable]
public class NodeGraphSaveData
{
    public List<NodeSaveData> nodes = new List<NodeSaveData>();
    public string graphID;

    public NodeGraphSaveData(string id)
    {
        graphID = id;
    }
}
