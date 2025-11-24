using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class UnlockedListManager : MonoBehaviour
{
    const string SAVE_FILE_NAME = "NodeSaveData.json";

    [Header("References")]
    [SerializeField] Transform unlockedListContainer;
    [SerializeField] GameObject unlockedNodeItemPrefab;
    [SerializeField] TextMeshProUGUI emptyListMessage;

    void Start()
    {
        LoadAndDisplayUnlockedNodes();
    }

    public void RefreshList()
    {
        LoadAndDisplayUnlockedNodes();
    }

    void LoadAndDisplayUnlockedNodes()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

        if (!File.Exists(saveFilePath))
        {
            ShowEmptyMessage("No saved data found. Unlock nodes in the Nodes Graph scene!");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        NodeSaveData saveData = JsonUtility.FromJson<NodeSaveData>(json);

        if (saveData == null || saveData.nodes.Count == 0)
        {
            ShowEmptyMessage("No saved data found. Unlock nodes in the Nodes Graph scene!");
            return;
        }

        List<NodeData> unlockedNodes = saveData.GetUnlockedNodes();

        if (unlockedNodes.Count == 0)
        {
            ShowEmptyMessage("No unlocked nodes yet. Visit the Nodes Graph scene and unlock some nodes!");
            return;
        }

        HideEmptyMessage();
        PopulateList(unlockedNodes);

        Debug.Log($"[Unlocked List] Displayed {unlockedNodes.Count} unlocked nodes");
    }

    void PopulateList(List<NodeData> unlockedNodes)
    {
        if (unlockedListContainer == null)
        {
            Debug.LogError("Unlocked list container is not assigned!");
            return;
        }

        if (unlockedNodeItemPrefab == null)
        {
            Debug.LogError("Unlocked node item prefab is not assigned!");
            return;
        }

        ClearList();

        foreach (NodeData nodeData in unlockedNodes)
        {
            GameObject itemObject = Instantiate(unlockedNodeItemPrefab, unlockedListContainer);
            UnlockedNodeItem item = itemObject.GetComponent<UnlockedNodeItem>();

            if (item != null)
            {
                item.Initialize(nodeData.nodeName, nodeData.nodeText);
            }
        }
    }

    void ClearList()
    {
        foreach (Transform child in unlockedListContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void ShowEmptyMessage(string message)
    {
        if (emptyListMessage != null)
        {
            emptyListMessage.text = message;
            emptyListMessage.gameObject.SetActive(true);
        }

        if (unlockedListContainer != null)
        {
            unlockedListContainer.gameObject.SetActive(false);
        }
    }

    void HideEmptyMessage()
    {
        if (emptyListMessage != null)
        {
            emptyListMessage.gameObject.SetActive(false);
        }

        if (unlockedListContainer != null)
        {
            unlockedListContainer.gameObject.SetActive(true);
        }
    }
}
