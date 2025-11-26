using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Node))]
public class NodeConnectionEditor : Editor
{
    static Node startNode = null;
    static bool isConnecting = false;

    const float NODE_RADIUS = 0.75f;
    const float LINE_WIDTH = 3f;

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (target == null || !(target is Node)) return;

        Node node = target as Node;
        if (node == null) return;

        Event e = Event.current;

        DrawExistingConnections(node);
        DrawNodeOutline(node);
        HandleDragConnection(node, e);
        HandleRightClickDisconnect(node, e);

        if (isConnecting)
        {
            SceneView.RepaintAll();
        }
    }

    void DrawExistingConnections(Node node)
    {
        Handles.color = new Color(1f, 0.8f, 0f, 0.8f);

        foreach (Node neighbor in node.neighborNodes)
        {
            if (neighbor != null)
            {
                Handles.DrawLine(node.transform.position, neighbor.transform.position, LINE_WIDTH);
            }
        }
    }

    void DrawNodeOutline(Node node)
    {
        if (isConnecting && startNode == node)
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(node.transform.position, Vector3.forward, NODE_RADIUS, 4f);
        }
        else
        {
            Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            Handles.DrawWireDisc(node.transform.position, Vector3.forward, NODE_RADIUS, 2f);
        }
    }

    void HandleDragConnection(Node node, Event e)
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
        {
            if (IsMouseOverNode(node, mouseWorldPos))
            {
                startNode = node;
                isConnecting = true;
                e.Use();
                Debug.Log($"Started dragging from {node.name}");
            }
        }

        if (isConnecting && startNode != null)
        {
            Handles.color = Color.green;
            Handles.DrawDottedLine(startNode.transform.position, mouseWorldPos, 3f);
        }

        if (e.type == EventType.MouseUp && e.button == 0 && isConnecting)
        {
            Node endNode = GetNodeUnderMouse(mouseWorldPos);

            if (endNode != null && endNode != startNode)
            {
                ConnectNodes(startNode, endNode);
            }

            startNode = null;
            isConnecting = false;
            e.Use();
            SceneView.RepaintAll();
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            startNode = null;
            isConnecting = false;
            e.Use();
            SceneView.RepaintAll();
        }
    }

    void HandleRightClickDisconnect(Node node, Event e)
    {
        if (e.type == EventType.MouseDown && e.button == 1)
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            Node targetNode = GetNodeUnderMouse(mouseWorldPos);

            if (targetNode != null && targetNode != node)
            {
                if (node.neighborNodes.Contains(targetNode))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent($"Disconnect from {targetNode.name}"), false, () =>
                    {
                        DisconnectNodes(node, targetNode);
                    });
                    menu.ShowAsContext();
                    e.Use();
                }
            }
        }
    }

    void ConnectNodes(Node nodeA, Node nodeB)
    {
        nodeA.AddNeighbor(nodeB);
        nodeB.AddNeighbor(nodeA);

        EditorUtility.SetDirty(nodeA);
        EditorUtility.SetDirty(nodeB);

        Debug.Log($"Connected: {nodeA.name} ↔ {nodeB.name}");
    }

    void DisconnectNodes(Node nodeA, Node nodeB)
    {
        nodeA.neighborNodes.Remove(nodeB);
        nodeB.neighborNodes.Remove(nodeA);

        EditorUtility.SetDirty(nodeA);
        EditorUtility.SetDirty(nodeB);

        Debug.Log($"Disconnected: {nodeA.name} ↮ {nodeB.name}");
        SceneView.RepaintAll();
    }

    bool IsMouseOverNode(Node node, Vector3 mouseWorldPos)
    {
        float distance = Vector3.Distance(mouseWorldPos, node.transform.position);
        return distance < NODE_RADIUS;
    }

    Node GetNodeUnderMouse(Vector3 mouseWorldPos)
    {
        Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);

        foreach (Node node in allNodes)
        {
            if (IsMouseOverNode(node, mouseWorldPos))
            {
                return node;
            }
        }

        return null;
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);
        
        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        
        return Vector3.zero;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Node node = (Node)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Node Connections", EditorStyles.boldLabel);

        if (node.neighborNodes.Count == 0)
        {
            EditorGUILayout.HelpBox("No connections.\n\nHold SHIFT + drag from this node to another in Scene view to connect.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.LabelField($"Connected to {node.neighborNodes.Count} node(s):", EditorStyles.miniLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            for (int i = node.neighborNodes.Count - 1; i >= 0; i--)
            {
                Node neighbor = node.neighborNodes[i];
                
                if (neighbor != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"→ {neighbor.name}", EditorStyles.miniLabel);
                    
                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        DisconnectNodes(node, neighbor);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    node.neighborNodes.RemoveAt(i);
                    EditorUtility.SetDirty(node);
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        if (isConnecting && startNode == node)
        {
            EditorGUILayout.HelpBox("Drag to another node to connect!", MessageType.Warning);
        }
    }
}
