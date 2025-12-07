using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeGraphController))]
public class NodeGraphControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NodeGraphController controller = (NodeGraphController)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Graph Controls", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        if (GUILayout.Button("Refresh Node Graph"))
        {
            controller.RefreshNodeGraph();
        }

        EditorGUILayout.EndVertical();
    }
}
