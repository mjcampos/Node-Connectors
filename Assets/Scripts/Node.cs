using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] string nodeID;
    
    public List<Node> neighborNodes = new List<Node>();
    
    public string NodeID => nodeID;

    void OnValidate()
    {
        GenerateNodeIDIfNeeded();
    }

    void Awake()
    {
        GenerateNodeIDIfNeeded();
    }

    public void OnNodeDataChanged()
    {
        RegenerateNodeID();
    }

    void GenerateNodeIDIfNeeded()
    {
        if (string.IsNullOrEmpty(nodeID))
        {
            RegenerateNodeID();
        }
    }

    void RegenerateNodeID()
    {
        NodeStateMachine stateMachine = GetComponent<NodeStateMachine>();
        
        if (stateMachine != null)
        {
            NodeDataSO nodeData = stateMachine.GetNodeData();
            
            if (nodeData != null)
            {
#if UNITY_EDITOR
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(nodeData);
                string dataGuid = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
                nodeID = $"{dataGuid}_{gameObject.name}";
#else
                nodeID = $"{nodeData.GetInstanceID()}_{gameObject.name}";
#endif
            }
            else
            {
                if (string.IsNullOrEmpty(nodeID))
                {
                    nodeID = System.Guid.NewGuid().ToString();
                }
            }
        }
        else
        {
            if (string.IsNullOrEmpty(nodeID))
            {
                nodeID = System.Guid.NewGuid().ToString();
            }
        }
    }

    public void AddNeighbor(Node node)
    {
        if (!neighborNodes.Contains(node))
        {
            neighborNodes.Add(node);
        }
    }
}
