
using Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TEST_KasperSAVe : MonoBehaviour
{
    private const string FILE_NAME = "FileName.es3";
    private const string KEY_NAME = "KeyToSave";

    [SerializeField] private List<NodeStateMachine> _nodeStateMachines;

    private string FullPath => Path.Combine(Application.persistentDataPath, FILE_NAME);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _nodeStateMachines = transform.GetComponentsInChildren<NodeStateMachine>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Save")]
    public void Save()
    {
        _nodeStateMachines = transform.GetComponentsInChildren<NodeStateMachine>().ToList();

        List<SaveNodeData> _nodeData = new List<SaveNodeData>();

        foreach (NodeStateMachine nodeStateMachine in _nodeStateMachines)
        {
            SaveNodeData snd = new SaveNodeData
            {                 
                state = nodeStateMachine.state,
                NodeID = nodeStateMachine.Node.NodeID
            };

            _nodeData.Add(snd);
        }

        Debug.Log("Awake → Checking for ES3 file at: " + FullPath);

        var settings = new ES3Settings(FILE_NAME);

        ES3.Save(KEY_NAME, _nodeData, settings);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        var settings = new ES3Settings(FILE_NAME);

        if (!ES3.FileExists(settings))
        {
            return;
        }

        List<SaveNodeData> _nodeData = ES3.Load<List<SaveNodeData>>(KEY_NAME, settings);

        foreach (NodeStateMachine nodeStateMachine in _nodeStateMachines)
        {
            foreach (SaveNodeData NodeData in _nodeData)
            {
                if(nodeStateMachine.Node.NodeID == NodeData.NodeID)
                {
                    nodeStateMachine.state = NodeData.state;
                }
            }

            nodeStateMachine.UpdateStateFromEnum();
        }

        // TODO Update the 
    }
}


public struct SaveNodeData 
{
    public NodeState state;
    public string NodeID;
}