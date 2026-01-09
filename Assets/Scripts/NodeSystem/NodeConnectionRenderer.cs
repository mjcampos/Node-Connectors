using Helpers;
using UnityEngine;
using Plugins.Radishmouse;

namespace NodeSystem
{
    [RequireComponent(typeof(Node))]
    public class NodeConnectionRenderer : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] GameObject edgePrefab;

        [Header("Line Settings")]
        [SerializeField] float lineWidth = 10f;
        [SerializeField] Color lineColor = Color.black;

        Node _node;
        NodeStateMachine _stateMachine;
        RectTransform _rectTransform;
        Canvas _canvas;
        RectTransform _canvasRectTransform;

        void Awake()
        {
            InitializeComponents();
        }

        void Start()
        {
            SyncConnections();
        }

        void OnEnable()
        {
            NodeStateMachine.OnNodeStateChanged += OnAnyNodeStateChanged;
        }

        void OnDisable()
        {
            NodeStateMachine.OnNodeStateChanged -= OnAnyNodeStateChanged;
        }

        void OnValidate()
        {
            InitializeComponents();
            
            if (Application.isPlaying)
            {
                SyncConnections();
            }
        }

        void InitializeComponents()
        {
            if (_node == null) _node = GetComponent<Node>();
            if (_stateMachine == null) _stateMachine = GetComponent<NodeStateMachine>();
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
            
            if (_canvas != null && _canvasRectTransform == null)
            {
                _canvasRectTransform = _canvas.GetComponent<RectTransform>();
            }
        }

        void OnAnyNodeStateChanged()
        {
            UpdateAllEdgesVisibility();
        }

        public void SyncConnections()
        {
            if (_node == null || edgePrefab == null) return;

            InitializeComponents();

            ClearAllEdges();

            foreach (Node neighbor in _node.neighborNodes)
            {
                if (neighbor == null) continue;

                if (ShouldCreateEdge(_node, neighbor))
                {
                    CreateEdge(neighbor);
                }
            }

            UpdateAllEdgesVisibility();
        }

        void CreateEdge(Node targetNode)
        {
            if (edgePrefab == null || targetNode == null) return;

            GameObject edgeInstance = Instantiate(edgePrefab, transform);
            
            edgeInstance.name = $"Edge_to_{targetNode.gameObject.name}";

            EdgeController edgeController = edgeInstance.GetComponent<EdgeController>();
            
            if (edgeController != null)
            {
                edgeController.Initialize(_rectTransform, targetNode.GetComponent<RectTransform>(), _canvasRectTransform);
            }
        }

        void ClearAllEdges()
        {
            EdgeController[] edges = GetComponentsInChildren<EdgeController>();
            foreach (EdgeController edge in edges)
            {
                if (edge != null)
                {
                    DestroyImmediate(edge.gameObject);
                }
            }
        }

        bool ShouldCreateEdge(Node nodeA, Node nodeB)
        {
            string idA = nodeA.NodeID;
            string idB = nodeB.NodeID;

            return string.Compare(idA, idB, System.StringComparison.Ordinal) < 0;
        }

        void UpdateAllEdgesVisibility()
        {
            if (_stateMachine == null) return;

            EdgeController[] edges = GetComponentsInChildren<EdgeController>();
            foreach (EdgeController edge in edges)
            {
                if (edge != null && edge.TargetRect != null)
                {
                    Node targetNode = edge.TargetRect.GetComponent<Node>();
                    if (targetNode != null)
                    {
                        NodeStateMachine targetStateMachine = targetNode.GetComponent<NodeStateMachine>();
                        if (targetStateMachine != null)
                        {
                            bool shouldShow = ShouldShowLine(_stateMachine, targetStateMachine);
                            edge.SetVisibility(shouldShow);

                            if (shouldShow)
                            {
                                Color edgeColor = GetLineColor(_stateMachine, targetStateMachine);
                                edge.SetColor(edgeColor);
                            }
                        }
                    }
                }
            }
        }

        bool ShouldShowLine(NodeStateMachine nodeA, NodeStateMachine nodeB)
        {
            bool nodeAIsHidden = IsNodeHidden(nodeA);
            bool nodeBIsHidden = IsNodeHidden(nodeB);
    
            if (nodeAIsHidden && nodeBIsHidden)
            {
                return false;
            }
    
            if (nodeAIsHidden)
            {
                bool nodeBIsVisible = IsNodeVisible(nodeB);
                return nodeA.degreesFromNonHoverable == 1 && nodeBIsVisible;
            }
    
            if (nodeBIsHidden)
            {
                bool nodeAIsVisible = IsNodeVisible(nodeA);
                return nodeB.degreesFromNonHoverable == 1 && nodeAIsVisible;
            }
    
            return true;
        }

        bool IsNodeHidden(NodeStateMachine node)
        {
            if (node.state == NodeState.Hidden)
            {
                return true;
            }

            if (node.state == NodeState.Locked)
            {
                return node.nodeImage != null && !node.nodeImage.enabled;
            }

            return false;
        }

        bool IsNodeVisible(NodeStateMachine node)
        {
            return node.state == NodeState.Visible || node.state == NodeState.NonHoverable || 
                   (node.state == NodeState.Locked && node.nodeImage != null && node.nodeImage.enabled);
        }

        Color GetLineColor(NodeStateMachine nodeA, NodeStateMachine nodeB)
        {
            Color fullColor = lineColor;

            bool nodeAIsHidden = IsNodeHidden(nodeA);
            bool nodeBIsHidden = IsNodeHidden(nodeB);

            if (nodeAIsHidden || nodeBIsHidden)
            {
                return new Color(fullColor.r, fullColor.g, fullColor.b, 0.5f);
            }

            return fullColor;
        }
    }
}
