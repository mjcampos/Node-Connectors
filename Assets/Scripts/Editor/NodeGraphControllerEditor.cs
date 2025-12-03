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
        EditorGUILayout.LabelField("Save System Controls", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Save Current State", GUILayout.Height(30)))
            {
                controller.SaveCurrentState();
            }

            if (GUILayout.Button("Load Saved State", GUILayout.Height(30)))
            {
                controller.LoadSavedState();
            }

            EditorGUILayout.Space(5);

            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Reset to Scene Default", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog(
                        "Reset to Default",
                        "This will reset all nodes to their scene setup and delete save data. Continue?",
                        "Reset",
                        "Cancel"))
                {
                    controller.ResetToDefaultState();
                }
            }
            GUI.backgroundColor = Color.white;
        }
        else
        {
            EditorGUILayout.HelpBox("Enter Play Mode to test save/load functionality.", MessageType.Info);
        }

        EditorGUILayout.EndVertical();

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
