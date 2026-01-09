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
        }
    }
}
