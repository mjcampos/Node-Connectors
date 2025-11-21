using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(SaveLoadManager))]
public class SaveLoadManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveLoadManager manager = (SaveLoadManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Save System Controls", EditorStyles.boldLabel);

        bool saveExists = manager.SaveFileExists();
        
        if (saveExists)
        {
            EditorGUILayout.HelpBox("Save file exists. Game will load saved progress on start.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("No save file. Game will use Editor defaults.", MessageType.Info);
        }

        EditorGUILayout.Space();

        GUI.backgroundColor = new Color(1f, 0.8f, 0.2f);
        if (GUILayout.Button("Revert to Default (Delete Save File)", GUILayout.Height(30)))
        {
            manager.RevertToDefault();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space();

        if (GUILayout.Button("Open Save File Location"))
        {
            string path = manager.GetSaveFilePath();
            string directory = Path.GetDirectoryName(path);
            
            if (Directory.Exists(directory))
            {
                EditorUtility.RevealInFinder(directory);
            }
            else
            {
                EditorUtility.DisplayDialog("Directory Not Found", 
                    $"Save directory does not exist yet:\n{directory}\n\nIt will be created when you first play the game.", 
                    "OK");
            }
        }

        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("How It Works", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "EDITOR MODE:\n" +
            "• Set your default node states here\n" +
            "• Changes made in Editor are your design defaults\n\n" +
            "PLAY MODE:\n" +
            "• Loads saved progress (if exists)\n" +
            "• Auto-saves when nodes change\n" +
            "• Progress persists between play sessions\n\n" +
            "TESTING:\n" +
            "• Click 'Revert to Default' to delete save data\n" +
            "• Next play session will use Editor defaults\n\n" +
            "Save Location: " + manager.GetSaveFilePath(),
            MessageType.None);
    }
}
