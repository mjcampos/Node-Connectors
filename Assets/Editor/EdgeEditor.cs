using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeGraphController))]
public class EdgeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        NodeGraphController nodeGraphController = (NodeGraphController)target;
        
        GUILayout.Space(10);
        
        // Show the button only when the object is selected and in Edit mode
        if (!Application.isPlaying && GUILayout.Button("Adjust Edges"))
        {
            nodeGraphController.TriggerEdgeAdjuster();
            EditorUtility.SetDirty(nodeGraphController);
        }
        
        GUILayout.Space(10);
        
        // Show the button only when the object is selected and in Edit mode
        if (!Application.isPlaying && GUILayout.Button("Correct Node Settings"))
        {
            nodeGraphController.TriggerNodeSettingsAdjuster();
            EditorUtility.SetDirty(nodeGraphController);
        }
    }
}
