using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace NodeSystem
{
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
            else
            {
#if UNITY_EDITOR
                ValidateUniqueID();
#endif
            }
        }

#if UNITY_EDITOR
        void ValidateUniqueID()
        {
            if (Application.isPlaying) return;

            Node[] allNodes = FindObjectsOfType<Node>();
            foreach (Node otherNode in allNodes)
            {
                if (otherNode != this && otherNode.nodeID == this.nodeID)
                {
                    Debug.LogWarning($"Duplicate NodeID detected on '{gameObject.name}'. Regenerating...", this);
                    RegenerateNodeID();
                    break;
                }
            }
        }
#endif

        void RegenerateNodeID()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                nodeID = System.Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
                Debug.Log($"Generated new NodeID for '{gameObject.name}': {nodeID}", this);
            }
            else
#endif
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

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null) return;

            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector3 center = (corners[0] + corners[2]) * 0.5f;

            Gizmos.color = new Color(0, 1, 1, 0.5f);
            Gizmos.DrawSphere(center, Vector3.Distance(corners[0], corners[2]) * 0.1f);
        }

        void OnDrawGizmosSelected()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null) return;

            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[3], corners[0]);
        }
#endif
    }
}
