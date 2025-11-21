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
        bool isPlaying = Application.isPlaying;
        
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
        
        string buttonText = isPlaying 
            ? "Revert to Default (Reset Nodes Now)" 
            : "Revert to Default (Delete Save File)";
        
        if (GUILayout.Button(buttonText, GUILayout.Height(35)))
        {
            if (isPlaying)
            {
                if (EditorUtility.DisplayDialog(
                    "Revert to Default?", 
                    "This will:\n\n• Delete the save file\n• Immediately reset all nodes to Editor defaults\n• Disable saving for this play session\n\nAre you sure?", 
                    "Yes, Revert", 
                    "Cancel"))
                {
                    manager.RevertToDefault();
                }
            }
            else
            {
                manager.RevertToDefault();
            }
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
        
        string helpText = isPlaying
            ? "IN PLAY MODE:\n" +
              "• Click 'Revert to Default' to reset nodes NOW\n" +
              "• Nodes will immediately return to Editor defaults\n" +
              "• Save file is deleted and saving is disabled\n\n"
            : "IN EDIT MODE:\n" +
              "• Click 'Revert to Default' to delete save file\n" +
              "• Next play session will use Editor defaults\n\n";

        helpText += "NORMAL WORKFLOW:\n" +
                   "• Edit Mode: Set default node states\n" +
                   "• Play Mode: Loads saved progress (if exists)\n" +
                   "• Auto-saves when nodes change\n" +
                   "• Progress persists between sessions\n\n" +
                   "Save Location: " + manager.GetSaveFilePath();

        EditorGUILayout.HelpBox(helpText, MessageType.None);
    }
}
