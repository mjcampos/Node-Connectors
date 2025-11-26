using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class NodeConnectionVisualizer
{
    const float LINE_WIDTH = 3f;
    static Color connectionColor = new Color(1f, 0.8f, 0f, 0.8f);

    static NodeConnectionVisualizer()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        DrawAllConnections();
    }

    static void DrawAllConnections()
    {
        Node[] allNodes = Object.FindObjectsByType<Node>(FindObjectsSortMode.None);

        Handles.color = connectionColor;

        foreach (Node node in allNodes)
        {
            if (node == null) continue;

            foreach (Node neighbor in node.neighborNodes)
            {
                if (neighbor == null) continue;

                int nodeID = node.GetInstanceID();
                int neighborID = neighbor.GetInstanceID();

                if (nodeID < neighborID)
                {
                    Handles.DrawLine(node.transform.position, neighbor.transform.position, LINE_WIDTH);
                }
            }
        }
    }
}
