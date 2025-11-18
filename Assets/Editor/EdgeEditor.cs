using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdjustAllEdges))]
public class EdgeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        AdjustAllEdges adjustAllEdges = (AdjustAllEdges)target;
        
        GUILayout.Space(10);
        
        // Show the button only when the object is selected and in Edit mode
        if (!Application.isPlaying && GUILayout.Button("Adjust Edges"))
        {
            adjustAllEdges.TriggerEdgeAdjuster();
            EditorUtility.SetDirty(adjustAllEdges);
        }
        
        GUILayout.Space(10);
        
        // Show the button only when the object is selected and in Edit mode
        if (!Application.isPlaying && GUILayout.Button("Correct Node Settings"))
        {
            adjustAllEdges.TriggerNodeSettingsAdjuster();
            EditorUtility.SetDirty(adjustAllEdges);
        }
    }
}
