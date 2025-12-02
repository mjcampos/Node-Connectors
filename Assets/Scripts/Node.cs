using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] string nodeID;
    
    public List<Node> neighborNodes = new List<Node>();
    
    public string NodeID => nodeID;

    void OnValidate()
    {
        GenerateNodeID();
    }

    void Awake()
    {
        if (string.IsNullOrEmpty(nodeID))
        {
            GenerateNodeID();
        }
    }

    public void OnNodeDataChanged()
    {
        GenerateNodeID();
    }

    void GenerateNodeID()
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
                nodeID = $"{dataGuid}_{gameObject.name}_{GetInstanceID()}";
#else
                nodeID = $"{nodeData.GetInstanceID()}_{gameObject.name}_{GetInstanceID()}";
#endif
            }
            else
            {
                nodeID = System.Guid.NewGuid().ToString();
            }
        }
        else
        {
            nodeID = System.Guid.NewGuid().ToString();
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
