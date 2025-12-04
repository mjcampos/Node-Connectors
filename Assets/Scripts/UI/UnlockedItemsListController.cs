using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Helpers;

public class UnlockedItemsListController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform contentParent;
    [SerializeField] GameObject unlockedItemRowPrefab;

    [Header("Save System")]
    [SerializeField] string graphID = "MainNodeGraph";

    void Start()
    {
        PopulateList();
    }

    public void PopulateList()
    {
        ClearList();

        NodeGraphSaveData saveData = LoadSaveData();
        if (saveData == null)
        {
            Debug.LogWarning("No save data found. List is empty.");
            return;
        }

        int unlockedCount = 0;

        foreach (NodeSaveData nodeData in saveData.nodes)
        {
            if (nodeData.state == NodeState.Unlocked)
            {
                SpawnRow(nodeData);
                unlockedCount++;
            }
        }

        Debug.Log($"Populated list with {unlockedCount} unlocked items.");
    }

    void SpawnRow(NodeSaveData nodeData)
    {
        if (unlockedItemRowPrefab == null || contentParent == null)
        {
            Debug.LogError("Missing references! Assign contentParent and unlockedItemRowPrefab.");
            return;
        }

        GameObject rowObj = Instantiate(unlockedItemRowPrefab, contentParent);
        UnlockedItemRow row = rowObj.GetComponent<UnlockedItemRow>();

        if (row != null)
        {
            string title = "Unknown";
            string description = "No description";

            NodeDataSO data = GetNodeDataFromGUID(nodeData.nodeDataGUID);
            if (data != null)
            {
                title = string.IsNullOrEmpty(data.title) ? "Untitled" : data.title;
                description = string.IsNullOrEmpty(data.description) ? "No description" : data.description;
            }

            row.SetData(nodeData.nodeID, title, description);
        }
    }

    void ClearList()
    {
        if (contentParent == null) return;

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    NodeGraphSaveData LoadSaveData()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "nodegraph_save.json");

        if (!File.Exists(savePath))
        {
            return null;
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<NodeGraphSaveData>(json);
    }

    NodeDataSO GetNodeDataFromGUID(string guid)
    {
        if (string.IsNullOrEmpty(guid))
            return null;

#if UNITY_EDITOR
        string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        if (!string.IsNullOrEmpty(assetPath))
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<NodeDataSO>(assetPath);
        }
#endif
        return null;
    }

    [ContextMenu("Refresh List")]
    public void RefreshList()
    {
        PopulateList();
    }
}
