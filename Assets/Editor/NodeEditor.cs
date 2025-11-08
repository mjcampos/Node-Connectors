using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        Node node = (Node)target;
        
        GUILayout.Space(10);
        
        // Show the button only when the object is selected and in Edit Mode
        if (!Application.isPlaying && GUILayout.Button("Open Node"))
        {
            node.CreateConnectionLines();
            EditorUtility.SetDirty(node);
        }
    }
}
