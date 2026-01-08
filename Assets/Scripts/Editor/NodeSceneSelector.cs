using UnityEngine;
using UnityEditor;
using NodeSystem;

namespace Editor
{
    [InitializeOnLoad]
    public class NodeSceneSelector
    {
        static NodeSceneSelector()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        static void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            
            if (e.type == EventType.Layout)
            {
                Node[] allNodes = Object.FindObjectsOfType<Node>();
                
                foreach (Node node in allNodes)
                {
                    if (node == null) continue;

                    RectTransform rectTransform = node.GetComponent<RectTransform>();
                    if (rectTransform == null) continue;

                    Vector3[] corners = new Vector3[4];
                    rectTransform.GetWorldCorners(corners);
                    Vector3 center = (corners[0] + corners[2]) * 0.5f;

                    float radius = Vector3.Distance(corners[0], corners[2]) * 0.5f;
                    
                    HandleUtility.AddControl(
                        node.GetInstanceID(),
                        HandleUtility.DistanceToCircle(center, radius)
                    );
                }
            }
            
            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                int controlID = HandleUtility.nearestControl;
                Node[] allNodes = Object.FindObjectsOfType<Node>();
                
                foreach (Node node in allNodes)
                {
                    if (node != null && node.GetInstanceID() == controlID)
                    {
                        Selection.activeGameObject = node.gameObject;
                        e.Use();
                        break;
                    }
                }
            }

            DrawNodeHandles();
        }

        static void DrawNodeHandles()
        {
            Node[] allNodes = Object.FindObjectsOfType<Node>();

            foreach (Node node in allNodes)
            {
                if (node == null) continue;

                RectTransform rectTransform = node.GetComponent<RectTransform>();
                if (rectTransform == null) continue;

                Vector3[] corners = new Vector3[4];
                rectTransform.GetWorldCorners(corners);
                Vector3 center = (corners[0] + corners[2]) * 0.5f;

                bool isSelected = Selection.activeGameObject == node.gameObject;
                
                Handles.color = isSelected ? new Color(1, 1, 0, 0.8f) : new Color(1, 1, 1, 0.3f);
                
                float size = Vector3.Distance(corners[0], corners[2]) * 0.5f;
                Handles.DrawWireDisc(center, Vector3.forward, size);
                
                if (isSelected)
                {
                    Handles.color = new Color(1, 1, 0, 0.2f);
                    Handles.DrawSolidDisc(center, Vector3.forward, size);
                }

                Handles.color = isSelected ? Color.yellow : new Color(0.8f, 0.8f, 0.8f, 0.5f);
                Handles.Label(center + Vector3.up * size * 1.2f, node.gameObject.name, EditorStyles.miniLabel);
            }
        }
    }
}
