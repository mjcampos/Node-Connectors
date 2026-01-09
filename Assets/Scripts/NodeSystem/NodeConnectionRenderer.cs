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

        Node _node;
        NodeStateMachine _stateMachine;
        RectTransform _rectTransform;
        Canvas _canvas;
        RectTransform _canvasRectTransform;
        Transform _edgesContainer;

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

            if (_edgesContainer == null)
            {
                _edgesContainer = FindEdgesContainer();
            }
        }

        Transform FindEdgesContainer()
        {
            Transform nodesParent = transform.parent;
            if (nodesParent == null)
            {
                Debug.LogError($"[NodeConnectionRenderer] {gameObject.name} has no parent. Cannot find Edges Container.");
                return null;
            }

            Transform container = nodesParent.Find("Edges Container");
            
            if (container == null)
            {
                Debug.LogError($"[NodeConnectionRenderer] 'Edges Container' not found under {nodesParent.name}. Please create it.");
            }
            
            return container;
        }

        void OnAnyNodeStateChanged()
        {
            UpdateAllEdgesVisibility();
        }

        public void SyncConnections()
        {
            if (_node == null || edgePrefab == null) return;

            InitializeComponents();

            if (_edgesContainer == null)
            {
                Debug.LogError($"[NodeConnectionRenderer] Edges Container not found. Cannot create edges.");
                return;
            }

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
            if (edgePrefab == null || targetNode == null || _edgesContainer == null) return;

            GameObject edgeInstance = Instantiate(edgePrefab, _edgesContainer);
            edgeInstance.name = $"Edge_{_node.name}_to_{targetNode.gameObject.name}";

            RectTransform edgeRect = edgeInstance.GetComponent<RectTransform>();
            if (edgeRect != null)
            {
                edgeRect.anchorMin = Vector2.zero;
                edgeRect.anchorMax = Vector2.one;
                edgeRect.sizeDelta = Vector2.zero;
                edgeRect.anchoredPosition = Vector2.zero;
            }

            UILineRenderer lineRenderer = edgeInstance.GetComponent<UILineRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.thickness = lineWidth;
                lineRenderer.raycastTarget = false;
                lineRenderer.center = false;
            }

            EdgeController edgeController = edgeInstance.GetComponent<EdgeController>();
            if (edgeController != null)
            {
                edgeController.Initialize(_rectTransform, targetNode.GetComponent<RectTransform>(), _canvasRectTransform);
            }
        }

        void ClearAllEdges()
        {
            if (_edgesContainer == null) return;

            EdgeController[] edges = _edgesContainer.GetComponentsInChildren<EdgeController>();
            foreach (EdgeController edge in edges)
            {
                if (edge != null && edge.StartRect == _rectTransform)
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
            if (_stateMachine == null || _edgesContainer == null) return;

            EdgeController[] edges = _edgesContainer.GetComponentsInChildren<EdgeController>();
            foreach (EdgeController edge in edges)
            {
                if (edge != null && edge.TargetRect != null && edge.StartRect != null)
                {
                    if (edge.StartRect != _rectTransform) continue;

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
                                float alpha = GetLineAlpha(_stateMachine, targetStateMachine);
                                edge.SetAlpha(alpha);
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

        float GetLineAlpha(NodeStateMachine nodeA, NodeStateMachine nodeB)
        {
            bool nodeAIsHidden = IsNodeHidden(nodeA);
            bool nodeBIsHidden = IsNodeHidden(nodeB);

            if (nodeAIsHidden || nodeBIsHidden)
            {
                return 0.5f;
            }

            return 1f;
        }
    }
}
