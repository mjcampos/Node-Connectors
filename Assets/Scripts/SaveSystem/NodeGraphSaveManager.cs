using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Helpers;

public class NodeGraphSaveManager : MonoBehaviour
{
    const string SAVE_FILE_NAME = "nodegraph_save.json";

    string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    [Header("Graph Settings")]
    [SerializeField] string graphID = "MainNodeGraph";

    NodeGraphController _graphController;

    void Awake()
    {
        _graphController = GetComponent<NodeGraphController>();
    }

    void Start()
    {
        LoadGame();
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        NodeGraphSaveData saveData = CaptureCurrentState();
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Game saved to: {SavePath}");
    }

    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found. Using scene default state.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        NodeGraphSaveData saveData = JsonUtility.FromJson<NodeGraphSaveData>(json);

        ApplySaveData(saveData);
        Debug.Log($"Game loaded from: {SavePath}");
    }

    public void ResetToDefault()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted.");
        }

        NodeStateMachine[] allStateMachines = GetComponentsInChildren<NodeStateMachine>();
        foreach (NodeStateMachine stateMachine in allStateMachines)
        {
            stateMachine.UpdateStateFromEnum();
            stateMachine.UpdateDegreesText();
        }

        _graphController?.TriggerNodeSettingsAdjuster();

        Debug.Log("Node graph reset to scene default state.");
    }

    NodeGraphSaveData CaptureCurrentState()
    {
        NodeGraphSaveData graphData = new NodeGraphSaveData(graphID);

        NodeStateMachine[] allStateMachines = GetComponentsInChildren<NodeStateMachine>();

        foreach (NodeStateMachine stateMachine in allStateMachines)
        {
            Node node = stateMachine.Node;
            if (node == null) continue;

            string dataGUID = "";
            NodeDataSO nodeData = stateMachine.GetNodeData();
        
            if (nodeData != null)
            {
#if UNITY_EDITOR
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(nodeData);
                dataGUID = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
#endif
            }

            NodeSaveData nodeSaveData = new NodeSaveData(
                node.NodeID,
                stateMachine.state,
                stateMachine.canBeUnlocked,
                stateMachine.degreesFromUnlocked,
                stateMachine.degreesFromVisible,
                stateMachine.degreesFromNonHoverable,
                dataGUID
            );

            graphData.nodes.Add(nodeSaveData);
        }

        return graphData;
    }

    void ApplySaveData(NodeGraphSaveData saveData)
    {
        if (saveData.graphID != graphID)
        {
            Debug.LogWarning($"Save data graph ID mismatch. Expected '{graphID}', got '{saveData.graphID}'");
            return;
        }

        Dictionary<string, NodeSaveData> nodeLookup = new Dictionary<string, NodeSaveData>();
        foreach (NodeSaveData nodeData in saveData.nodes)
        {
            nodeLookup[nodeData.nodeID] = nodeData;
        }

        NodeStateMachine[] allStateMachines = GetComponentsInChildren<NodeStateMachine>();

        foreach (NodeStateMachine stateMachine in allStateMachines)
        {
            Node node = stateMachine.Node;
            if (node == null) continue;

            if (nodeLookup.TryGetValue(node.NodeID, out NodeSaveData nodeData))
            {
                stateMachine.state = nodeData.state;
                stateMachine.canBeUnlocked = nodeData.canBeUnlocked;
                stateMachine.degreesFromUnlocked = nodeData.degreesFromUnlocked;
                stateMachine.degreesFromVisible = nodeData.degreesFromVisible;
                stateMachine.degreesFromNonHoverable = nodeData.degreesFromNonHoverable;

                stateMachine.UpdateStateFromEnum();
                stateMachine.UpdateDegreesText();
            }
        }

        Debug.Log($"Applied save data for {saveData.nodes.Count} nodes.");
    }

    public string GetSaveFilePath()
    {
        return SavePath;
    }
}
