using Helpers;
using NodeSystem;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Node))]
    public class NodeConnectionEditor : UnityEditor.Editor
    {
        static Node startNode = null;
        static bool isDragging = false;
        static Vector2 currentMousePos;
        static bool debugMode = false;

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
            Event e = Event.current;
            
            currentMousePos = e.mousePosition;

            DrawDragLine();
            
            if (debugMode)
            {
                DrawDebugInfo();
            }
            
            HandleRightClickDisconnect(e);
            
            if (!e.shift)
            {
                if (isDragging)
                {
                    isDragging = false;
                    startNode = null;
                }
                return;
            }

            HandleInput(e);

            if (isDragging)
            {
                SceneView.RepaintAll();
            }
        }

        void DrawDebugInfo()
        {
            Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
            
            Ray ray = HandleUtility.GUIPointToWorldRay(currentMousePos);
            Plane plane = new Plane(Vector3.forward, Vector3.zero);
            
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);
                
                Handles.color = Color.magenta;
                Handles.DrawWireCube(worldPoint, Vector3.one * 0.1f);
                Handles.Label(worldPoint + Vector3.up * 0.15f, $"Mouse: ({worldPoint.x:F2}, {worldPoint.y:F2})");
                
                foreach (Node node in allNodes)
                {
                    RectTransform rectTransform = node.GetComponent<RectTransform>();
                    if (rectTransform == null) continue;
                    
                    Vector3[] corners = new Vector3[4];
                    rectTransform.GetWorldCorners(corners);
                    
                    Handles.color = Color.cyan;
                    Handles.DrawLine(corners[0], corners[1]);
                    Handles.DrawLine(corners[1], corners[2]);
                    Handles.DrawLine(corners[2], corners[3]);
                    Handles.DrawLine(corners[3], corners[0]);
                    
                    Vector3 center = (corners[0] + corners[2]) * 0.5f;
                    Handles.Label(center, $"{node.name}\nMin: ({corners[0].x:F2}, {corners[0].y:F2})\nMax: ({corners[2].x:F2}, {corners[2].y:F2})");
                }
            }
        }

        void HandleRightClickDisconnect(Event e)
        {
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                Node selectedNode = target as Node;
                if (selectedNode == null) return;

                Node clickedNode = GetNodeUnderMouse(e.mousePosition);
                
                if (clickedNode != null && clickedNode != selectedNode)
                {
                    if (selectedNode.neighborNodes.Contains(clickedNode))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent($"Disconnect {selectedNode.name} from {clickedNode.name}"), false, () =>
                        {
                            DisconnectNodes(selectedNode, clickedNode);
                        });
                        menu.ShowAsContext();
                        e.Use();
                    }
                }
            }
        }

        void HandleInput(Event e)
        {
            if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
            {
                Node nodeUnderMouse = GetNodeUnderMouse(e.mousePosition);
                
                if (debugMode)
                {
                    Debug.Log($"Mouse down - Node found: {(nodeUnderMouse != null ? nodeUnderMouse.name : "NULL")}");
                }
                
                if (nodeUnderMouse != null)
                {
                    startNode = nodeUnderMouse;
                    isDragging = true;
                    e.Use();
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Debug.Log($"✅ Started dragging from: {startNode.name}");
                }
            }

            if (e.type == EventType.MouseDrag && isDragging)
            {
                e.Use();
                SceneView.RepaintAll();
            }

            if (e.type == EventType.MouseUp && e.button == 0 && isDragging)
            {
                Node endNode = GetNodeUnderMouse(e.mousePosition);
                
                if (endNode != null && endNode != startNode)
                {
                    ConnectNodes(startNode, endNode);
                }
                
                isDragging = false;
                startNode = null;
                GUIUtility.hotControl = 0;
                e.Use();
                SceneView.RepaintAll();
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape && isDragging)
            {
                isDragging = false;
                startNode = null;
                GUIUtility.hotControl = 0;
                e.Use();
                SceneView.RepaintAll();
            }

            if (e.shift && !isDragging)
            {
                if (e.type == EventType.Layout)
                {
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                }
            }
        }

        void DrawDragLine()
        {
            if (!isDragging || startNode == null) return;

            Vector3 startWorldPos = GetNodeWorldPosition(startNode);
            
            Ray ray = HandleUtility.GUIPointToWorldRay(currentMousePos);
            Plane plane = new Plane(Vector3.forward, startWorldPos);
            
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 mouseWorldPos = ray.GetPoint(distance);
                
                Handles.color = Color.green;
                Handles.DrawDottedLine(startWorldPos, mouseWorldPos, 3f);
                
                Node nodeUnderMouse = GetNodeUnderMouse(currentMousePos);
                if (nodeUnderMouse != null && nodeUnderMouse != startNode)
                {
                    RectTransform rectTransform = nodeUnderMouse.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        Vector3[] corners = new Vector3[4];
                        rectTransform.GetWorldCorners(corners);
                        
                        Handles.color = new Color(0, 1, 0, 0.3f);
                        Handles.DrawSolidRectangleWithOutline(corners, new Color(0, 1, 0, 0.2f), Color.green);
                    }
                }
            }
        }

        Node GetNodeUnderMouse(Vector2 mouseGUIPos)
        {
            Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
            
            Ray ray = HandleUtility.GUIPointToWorldRay(mouseGUIPos);
            
            Node closestNode = null;
            float closestDistance = float.MaxValue;
            
            foreach (Node node in allNodes)
            {
                RectTransform rectTransform = node.GetComponent<RectTransform>();
                if (rectTransform == null) continue;
                
                Vector3[] corners = new Vector3[4];
                rectTransform.GetWorldCorners(corners);
                
                Vector3 min = new Vector3(
                    Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x),
                    Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y),
                    Mathf.Min(corners[0].z, corners[1].z, corners[2].z, corners[3].z)
                );
                
                Vector3 max = new Vector3(
                    Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x),
                    Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y),
                    Mathf.Max(corners[0].z, corners[1].z, corners[2].z, corners[3].z)
                );
                
                Bounds bounds = new Bounds();
                bounds.SetMinMax(min, max);
                
                if (bounds.IntersectRay(ray, out float hitDistance))
                {
                    if (hitDistance < closestDistance)
                    {
                        closestDistance = hitDistance;
                        closestNode = node;
                    }
                }
            }
            
            if (debugMode && closestNode != null)
            {
                Debug.Log($"GetNodeUnderMouse found: {closestNode.name}");
            }
            
            return closestNode;
        }

        Vector3 GetNodeWorldPosition(Node node)
        {
            RectTransform rectTransform = node.GetComponent<RectTransform>();
            
            if (rectTransform == null)
            {
                return node.transform.position;
            }
            
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector3 center = (corners[0] + corners[2]) * 0.5f;
            
            return center;
        }

        void ConnectNodes(Node nodeA, Node nodeB)
        {
            if (nodeA.neighborNodes.Contains(nodeB))
            {
                Debug.Log($"ℹ️ Nodes already connected: {nodeA.name} ↔ {nodeB.name}");
                return;
            }

            nodeA.AddNeighbor(nodeB);
            nodeB.AddNeighbor(nodeA);

            EditorUtility.SetDirty(nodeA);
            EditorUtility.SetDirty(nodeB);

            Debug.Log($"✅ Connected: {nodeA.name} ↔ {nodeB.name}");

            // NEW: Sync Edge prefabs in editor
            NodeConnectionRenderer rendererA = nodeA.GetComponent<NodeConnectionRenderer>();
            NodeConnectionRenderer rendererB = nodeB.GetComponent<NodeConnectionRenderer>();
            
            if (rendererA != null) rendererA.SyncConnections();
            if (rendererB != null) rendererB.SyncConnections();

            RippleAllUnlockedNodes();
            SceneView.RepaintAll();
        }

        void DisconnectNodes(Node nodeA, Node nodeB)
        {
            nodeA.neighborNodes.Remove(nodeB);
            nodeB.neighborNodes.Remove(nodeA);

            EditorUtility.SetDirty(nodeA);
            EditorUtility.SetDirty(nodeB);

            Debug.Log($"🔗 Disconnected: {nodeA.name} ↮ {nodeB.name}");

            // NEW: Sync Edge prefabs in editor
            NodeConnectionRenderer rendererA = nodeA.GetComponent<NodeConnectionRenderer>();
            NodeConnectionRenderer rendererB = nodeB.GetComponent<NodeConnectionRenderer>();
            
            if (rendererA != null) rendererA.SyncConnections();
            if (rendererB != null) rendererB.SyncConnections();

            RippleAllUnlockedNodes();
            SceneView.RepaintAll();
        }

        void RippleAllUnlockedNodes()
        {
            NodeStateMachine[] allStateMachines = FindObjectsByType<NodeStateMachine>(FindObjectsSortMode.None);
    
            foreach (NodeStateMachine stateMachine in allStateMachines)
            {
                if (stateMachine.state == NodeState.Visible || 
                    stateMachine.state == NodeState.NonHoverable || 
                    stateMachine.state == NodeState.Hidden)
                {
                    stateMachine.degreesFromUnlocked = int.MaxValue;
                }
            }
    
            foreach (NodeStateMachine stateMachine in allStateMachines)
            {
                if (stateMachine.state == NodeState.Unlocked)
                {
                    stateMachine.Ripple();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Node node = (Node)target;

            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Connection Settings", EditorStyles.boldLabel);
            
            debugMode = EditorGUILayout.Toggle("Debug Mode", debugMode);
            
            if (debugMode)
            {
                EditorGUILayout.HelpBox("Debug mode enabled. Check Scene View for visual bounds and Console for logs.", MessageType.Warning);
            }
            
            EditorGUILayout.HelpBox("✨ HOW TO USE:\n• CONNECT: Hold SHIFT + click and drag anywhere within a node, then release on another node\n• DISCONNECT: Select a node, then right-click a connected node", MessageType.Info);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Node Connections", EditorStyles.boldLabel);

            if (node.neighborNodes.Count == 0)
            {
                EditorGUILayout.LabelField("No connections yet", EditorStyles.miniLabel);
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
                    
                        if (GUILayout.Button("Disconnect", GUILayout.Width(80)))
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

            if (isDragging && startNode == node)
            {
                EditorGUILayout.HelpBox("🟢 Dragging... Release on target node to connect!", MessageType.Warning);
            }
            
            if (GUILayout.Button("Refresh Scene View"))
            {
                SceneView.RepaintAll();
            }
        }
    }
}
