using UnityEngine;
using Helpers;

[RequireComponent(typeof(Node))]
public class NodeConnectionRenderer : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] float lineWidth = 0.1f;

    [SerializeField] Color lineColor = Color.black;
    [SerializeField] Material lineMaterial;
    [SerializeField] int sortingOrder = -1;

    Node _node;
    NodeStateMachine _stateMachine;
    LineRenderer[] _lineRenderers;
    Node[] _lineNeighbors;

    void Awake()
    {
        _node = GetComponent<Node>();
        _stateMachine = GetComponent<NodeStateMachine>();
    }

    void Start()
    {
        CreateLineRenderers();
        UpdateLineVisibility();
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
        if (Application.isPlaying && _lineRenderers != null)
        {
            UpdateLineAppearance();
        }
    }

    void OnAnyNodeStateChanged()
    {
        UpdateLineVisibility();
    }

    void CreateLineRenderers()
    {
        if (_node == null) return;

        ClearExistingLines();

        int connectionCount = 0;
        foreach (Node neighbor in _node.neighborNodes)
        {
            if (neighbor == null) continue;

            int nodeID = gameObject.GetInstanceID();
            int neighborID = neighbor.gameObject.GetInstanceID();

            if (nodeID < neighborID)
            {
                connectionCount++;
            }
        }

        _lineRenderers = new LineRenderer[connectionCount];
        _lineNeighbors = new Node[connectionCount];
        int index = 0;

        foreach (Node neighbor in _node.neighborNodes)
        {
            if (neighbor == null) continue;

            int nodeID = gameObject.GetInstanceID();
            int neighborID = neighbor.gameObject.GetInstanceID();

            if (nodeID < neighborID)
            {
                GameObject lineObj = new GameObject($"Line_{gameObject.name}_to_{neighbor.gameObject.name}");
                lineObj.transform.SetParent(transform);
                lineObj.transform.localPosition = Vector3.zero;

                LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.startWidth = lineWidth;
                lr.endWidth = lineWidth;
                lr.startColor = lineColor;
                lr.endColor = lineColor;
                lr.sortingOrder = sortingOrder;
                lr.useWorldSpace = true;

                if (lineMaterial != null)
                {
                    lr.material = lineMaterial;
                }
                else
                {
                    lr.material = new Material(Shader.Find("Sprites/Default"));
                }

                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, neighbor.transform.position);

                _lineRenderers[index] = lr;
                _lineNeighbors[index] = neighbor;
                index++;
            }
        }
    }

    void LateUpdate()
    {
        UpdateLinePositions();
    }

    void UpdateLinePositions()
    {
        if (_lineRenderers == null || _lineNeighbors == null) return;

        for (int i = 0; i < _lineRenderers.Length; i++)
        {
            if (_lineRenderers[i] != null && _lineNeighbors[i] != null)
            {
                _lineRenderers[i].SetPosition(0, transform.position);
                _lineRenderers[i].SetPosition(1, _lineNeighbors[i].transform.position);
            }
        }
    }

    void UpdateLineVisibility()
    {
        if (_lineRenderers == null || _lineNeighbors == null || _stateMachine == null) return;

        for (int i = 0; i < _lineRenderers.Length; i++)
        {
            if (_lineRenderers[i] != null && _lineNeighbors[i] != null)
            {
                NodeStateMachine neighborStateMachine = _lineNeighbors[i].GetComponent<NodeStateMachine>();
                
                if (neighborStateMachine != null)
                {
                    bool shouldShowLine = ShouldShowLine(_stateMachine, neighborStateMachine);
                    _lineRenderers[i].enabled = shouldShowLine;
                    
                    if (shouldShowLine)
                    {
                        UpdateLineColor(_lineRenderers[i], _stateMachine, neighborStateMachine);
                    }
                }
            }
        }
    }

    bool ShouldShowLine(NodeStateMachine nodeA, NodeStateMachine nodeB)
    {
        bool nodeAIsHidden = nodeA.state == NodeState.Hidden;
        bool nodeBIsHidden = nodeB.state == NodeState.Hidden;
        
        if (nodeAIsHidden && nodeBIsHidden)
        {
            return false;
        }
        
        if (nodeAIsHidden)
        {
            return nodeA.degreesFromNonHoverable == 1;
        }
        
        if (nodeBIsHidden)
        {
            return nodeB.degreesFromNonHoverable == 1;
        }
        
        return true;
    }

    void UpdateLineColor(LineRenderer lr, NodeStateMachine nodeA, NodeStateMachine nodeB)
    {
        Color fullColor = lineColor;
        Color transparentColor = new Color(lineColor.r, lineColor.g, lineColor.b, 0f);

        if (nodeA.state == NodeState.Hidden && nodeA.degreesFromNonHoverable == 1)
        {
            lr.startColor = transparentColor;
            lr.endColor = fullColor;
        }
        else if (nodeB.state == NodeState.Hidden && nodeB.degreesFromNonHoverable == 1)
        {
            lr.startColor = fullColor;
            lr.endColor = transparentColor;
        }
        else
        {
            lr.startColor = fullColor;
            lr.endColor = fullColor;
        }
    }

    void UpdateLineAppearance()
    {
        if (_lineRenderers == null) return;

        foreach (LineRenderer lr in _lineRenderers)
        {
            if (lr != null)
            {
                lr.startWidth = lineWidth;
                lr.endWidth = lineWidth;
                lr.sortingOrder = sortingOrder;

                if (lineMaterial != null)
                {
                    lr.material = lineMaterial;
                }
            }
        }
        
        UpdateLineVisibility();
    }

    void ClearExistingLines()
    {
        if (_lineRenderers != null)
        {
            foreach (LineRenderer lr in _lineRenderers)
            {
                if (lr != null)
                {
                    Destroy(lr.gameObject);
                }
            }
        }

        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Line_"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    void OnDestroy()
    {
        ClearExistingLines();
    }
}
