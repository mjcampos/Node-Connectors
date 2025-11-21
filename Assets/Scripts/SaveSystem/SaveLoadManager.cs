using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    const string SAVE_FILE_NAME = "NodeSaveData.json";
    
    [Header("References")]
    [SerializeField] NodeGraphController nodeGraphController;

    string _saveFilePath;
    bool _saveOnQuit = true;

    void Awake()
    {
        if (nodeGraphController == null)
        {
            nodeGraphController = GetComponent<NodeGraphController>();
        }

        _saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        
        Debug.Log($"Save file location: {_saveFilePath}");
    }

    void OnEnable()
    {
        NodeStateMachine.OnNodeStateChanged += OnNodeChanged;
    }

    void OnDisable()
    {
        NodeStateMachine.OnNodeStateChanged -= OnNodeChanged;
    }

    void Start()
    {
        LoadNodeStates();
    }

    void OnApplicationQuit()
    {
        if (_saveOnQuit)
        {
            SaveNodeStates();
        }
        else
        {
            Debug.Log("[Save System] Skipping save on quit (reverted to default)");
        }
    }

    void OnNodeChanged()
    {
        if (Application.isPlaying && _saveOnQuit)
        {
            SaveNodeStates();
        }
    }

    public void SaveNodeStates()
    {
        if (nodeGraphController == null)
        {
            Debug.LogWarning("NodeGraphController is not assigned!");
            return;
        }

        NodeSaveData saveData = new NodeSaveData();

        foreach (Transform child in nodeGraphController.transform)
        {
            NodeStateMachine nsm = child.GetComponent<NodeStateMachine>();
            
            if (nsm == null) continue;

            NodeData data = new NodeData
            {
                nodeName = child.name,
                state = nsm.state,
                canBeUnlocked = nsm.canBeUnlocked,
                degreesFromUnlocked = nsm.degreesFromUnlocked,
                degreesFromVisible = nsm.degreesFromVisible,
                degreesFromNonHoverable = nsm.degreesFromNonHoverable
            };

            saveData.AddOrUpdateNode(data);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(_saveFilePath, json);

        Debug.Log($"[Save System] Saved {saveData.nodes.Count} nodes");
    }

    public void LoadNodeStates()
    {
        if (!File.Exists(_saveFilePath))
        {
            Debug.Log("[Save System] No save file found. Starting with Editor defaults.");
            return;
        }

        if (nodeGraphController == null)
        {
            Debug.LogWarning("NodeGraphController is not assigned!");
            return;
        }

        string json = File.ReadAllText(_saveFilePath);
        NodeSaveData saveData = JsonUtility.FromJson<NodeSaveData>(json);

        if (saveData == null || saveData.nodes.Count == 0)
        {
            Debug.Log("[Save System] Save file is empty. Starting with Editor defaults.");
            return;
        }

        int loadedCount = 0;
        
        foreach (Transform child in nodeGraphController.transform)
        {
            NodeStateMachine nsm = child.GetComponent<NodeStateMachine>();
            
            if (nsm == null) continue;

            NodeData data = saveData.FindNodeData(child.name);
            
            if (data != null)
            {
                nsm.state = data.state;
                nsm.canBeUnlocked = data.canBeUnlocked;
                nsm.degreesFromUnlocked = data.degreesFromUnlocked;
                nsm.degreesFromVisible = data.degreesFromVisible;
                nsm.degreesFromNonHoverable = data.degreesFromNonHoverable;
                nsm.UpdateStateFromEnum();
                loadedCount++;
            }
        }

        Debug.Log($"[Save System] Loaded {loadedCount} nodes from save file");
    }

    public void RevertToDefault()
    {
        if (File.Exists(_saveFilePath))
        {
            File.Delete(_saveFilePath);
            _saveOnQuit = false;
            
            Debug.Log("[Save System] Save file deleted. Save disabled for this session.");
            
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog(
                "Save Data Deleted", 
                "Save file has been deleted and saving is disabled for this play session.\n\nNext time you enter Play Mode, nodes will start from their Editor defaults.", 
                "OK");
            #endif
        }
        else
        {
            Debug.Log("[Save System] No save file found. Already using Editor defaults.");
            
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog(
                "No Save Data", 
                "There is no save file to delete.\n\nNodes are already using their Editor defaults.", 
                "OK");
            #endif
        }
    }

    public string GetSaveFilePath()
    {
        return _saveFilePath;
    }

    public bool SaveFileExists()
    {
        return File.Exists(_saveFilePath);
    }
}
