using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    const string SAVE_FILE_NAME = "NodeSaveData.json";
    
    [Header("References")]
    [SerializeField] NodeGraphController nodeGraphController;

    string _saveFilePath;
    bool _saveOnQuit = true;
    NodeSaveData _defaultNodeStates;

    void Awake()
    {
        if (nodeGraphController == null)
        {
            nodeGraphController = GetComponent<NodeGraphController>();
        }

        InitializeSavePath();
        
        Debug.Log($"Save file location: {_saveFilePath}");
    }

    void InitializeSavePath()
    {
        if (string.IsNullOrEmpty(_saveFilePath))
            _saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
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
        CaptureDefaultStates();
        LoadNodeStates();
        SaveNodeStates();
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

    void CaptureDefaultStates()
    {
        if (nodeGraphController == null)
        {
            Debug.LogWarning("NodeGraphController is not assigned!");
            return;
        }

        _defaultNodeStates = new NodeSaveData();

        foreach (Transform child in nodeGraphController.transform)
        {
            NodeStateMachine nsm = child.GetComponent<NodeStateMachine>();
            UIController uiController = child.GetComponent<UIController>();
            
            if (nsm == null) continue;

            NodeData data = new NodeData
            {
                nodeName = child.name,
                state = nsm.state,
                canBeUnlocked = nsm.canBeUnlocked,
                degreesFromUnlocked = nsm.degreesFromUnlocked,
                degreesFromVisible = nsm.degreesFromVisible,
                degreesFromNonHoverable = nsm.degreesFromNonHoverable,
                nodeText = (uiController != null && uiController.HasHoverText()) ? uiController.GetHoverText() : string.Empty
            };

            _defaultNodeStates.AddOrUpdateNode(data);
        }

        Debug.Log($"[Save System] Captured {_defaultNodeStates.nodes.Count} default node states from Editor");
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
            UIController uiController = child.GetComponent<UIController>();
        
            if (nsm == null) continue;

            NodeData data = new NodeData
            {
                nodeName = child.name,
                state = nsm.state,
                canBeUnlocked = nsm.canBeUnlocked,
                degreesFromUnlocked = nsm.degreesFromUnlocked,
                degreesFromVisible = nsm.degreesFromVisible,
                degreesFromNonHoverable = nsm.degreesFromNonHoverable,
                nodeText = (uiController != null && uiController.HasHoverText()) ? uiController.GetHoverText() : string.Empty
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

        ApplyNodeData(saveData);

        Debug.Log($"[Save System] Loaded {saveData.nodes.Count} nodes from save file");
    }

    public void RestoreDefaultStates()
    {
        if (_defaultNodeStates == null || _defaultNodeStates.nodes.Count == 0)
        {
            Debug.LogWarning("[Save System] No default states captured!");
            return;
        }

        ApplyNodeData(_defaultNodeStates);

        Debug.Log($"[Save System] Restored {_defaultNodeStates.nodes.Count} nodes to Editor defaults");
    }

    void ApplyNodeData(NodeSaveData saveData)
    {
        if (nodeGraphController == null) return;

        int appliedCount = 0;
        
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
                appliedCount++;
            }
        }

        Debug.Log($"[Save System] Applied data to {appliedCount} nodes");
    }

    public void RevertToDefault()
    {
        InitializeSavePath();
        
        bool wasInPlayMode = Application.isPlaying;
        bool fileExisted = File.Exists(_saveFilePath);

        if (fileExisted)
        {
            File.Delete(_saveFilePath);
            Debug.Log($"[Save System] Save file deleted: {_saveFilePath}");
        }

        if (wasInPlayMode)
        {
            _saveOnQuit = false;
            RestoreDefaultStates();
            
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog(
                "Reverted to Default", 
                "Nodes have been restored to their Editor default states.\n\nSave file deleted and saving disabled for this session.", 
                "OK");
            #endif
        }
        else
        {
            #if UNITY_EDITOR
            if (fileExisted)
            {
                UnityEditor.EditorUtility.DisplayDialog(
                    "Save Data Deleted", 
                    "Save file has been deleted.\n\nNext time you enter Play Mode, nodes will start from their Editor defaults.", 
                    "OK");
            }
            else
            {
                UnityEditor.EditorUtility.DisplayDialog(
                    "No Save Data", 
                    "There is no save file to delete.\n\nNodes are already using their Editor defaults.", 
                    "OK");
            }
            #endif
        }
    }

    public string GetSaveFilePath()
    {
        InitializeSavePath();
        return _saveFilePath;
    }

    public bool SaveFileExists()
    {
        InitializeSavePath();
        return File.Exists(_saveFilePath);
    }
}
