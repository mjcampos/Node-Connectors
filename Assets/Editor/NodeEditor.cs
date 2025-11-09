using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EdgeGenerator))]
public class NodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EdgeGenerator edgeGenerator = (EdgeGenerator)target;
        
        GUILayout.Space(10);
        
        // Show the button only when the object is selected and in Edit Mode
        if (!Application.isPlaying && GUILayout.Button("Adjust Edges"))
        {
            edgeGenerator.AdjustEdges();
            EditorUtility.SetDirty(edgeGenerator);
        }
    }
}
