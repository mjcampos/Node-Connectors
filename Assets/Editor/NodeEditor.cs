using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdjacentNodes))]
public class NodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        AdjacentNodes adjacentNodes = (AdjacentNodes)target;
        
        GUILayout.Space(10);
        
        // Show the button only when the object is selected and in Edit Mode
        if (!Application.isPlaying && GUILayout.Button("Adjust Edges"))
        {
            adjacentNodes.AdjustEdges();
            EditorUtility.SetDirty(adjacentNodes);
        }
    }
}
