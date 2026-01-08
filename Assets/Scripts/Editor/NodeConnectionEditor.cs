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

        const string PREF_KEY_RADIUS = "NodeConnectionEditor_DetectionRadius";
        const float DEFAULT_RADIUS = 50f;

        static float nodeRadius = -1f;

        static float NodeRadius
        {
            get
            {
                if (nodeRadius < 0)
                {
                    nodeRadius = EditorPrefs.GetFloat(PREF_KEY_RADIUS, DEFAULT_RADIUS);
                }
                return nodeRadius;
            }
            
            set
            {
                nodeRadius = value;
                EditorPrefs.SetFloat(PREF_KEY_RADIUS, value);
            }
        }

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

            DrawExistingConnections();
            DrawDragLine();
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

        void DrawExistingConnections()
        {
            Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
            
            Handles.color = new Color(1f, 0.8f, 0f, 0.8f);
            
            foreach (Node node in allNodes)
            {
                if (node.neighborNodes == null) continue;
                
                foreach (Node neighbor in node.neighborNodes)
                {
                    if (neighbor != null && neighbor.GetInstanceID() > node.GetInstanceID())
                    {
                        Vector3 startPos = GetNodeWorldPosition(node);
                        Vector3 endPos = GetNodeWorldPosition(neighbor);
                        
                        Handles.DrawLine(startPos, endPos, 3f);
                    }
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
                    Vector3 targetPos = GetNodeWorldPosition(nodeUnderMouse);
                    Handles.DrawWireDisc(targetPos, Vector3.forward, NodeRadius, 4f);
                }
            }
        }

        Node GetNodeUnderMouse(Vector2 mouseGUIPos)
        {
            Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
            
            Ray ray = HandleUtility.GUIPointToWorldRay(mouseGUIPos);
            Plane plane = new Plane(Vector3.forward, Vector3.zero);
            
            if (!plane.Raycast(ray, out float distance))
            {
                return null;
            }
            
            Vector3 worldPoint = ray.GetPoint(distance);
            
            float closestDistance = float.MaxValue;
            Node closestNode = null;
            
            foreach (Node node in allNodes)
            {
                Vector3 nodeWorldPos = GetNodeWorldPosition(node);
                float dist = Vector3.Distance(worldPoint, nodeWorldPos);
                
                if (dist < NodeRadius && dist < closestDistance)
                {
                    closestDistance = dist;
                    closestNode = node;
                }
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
            
            EditorGUI.BeginChangeCheck();
            float newRadius = EditorGUILayout.Slider("Detection Radius", NodeRadius, 20f, 200f);
            if (EditorGUI.EndChangeCheck())
            {
                NodeRadius = newRadius;
                SceneView.RepaintAll();
            }
            
            EditorGUILayout.HelpBox($"Detection radius: {NodeRadius:F0} units\n\n✨ HOW TO USE:\n• CONNECT: Hold SHIFT + drag from node to node\n• DISCONNECT: Select a node, then right-click a connected node", MessageType.Info);

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
            
            EditorGUILayout.Space(5);
            
            if (GUILayout.Button("Reset Detection Radius to Default"))
            {
                NodeRadius = DEFAULT_RADIUS;
                SceneView.RepaintAll();
            }
        }
    }
}
