using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class NodeAutoParenting
{
    const string NODE_GRAPH_SCENE_NAME = "Nodes Graph";
    const string NODES_WRAPPER_NAME = "Nodes";

    static NodeAutoParenting()
    {
        ObjectChangeEvents.changesPublished += OnObjectChanges;
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    static void OnObjectChanges(ref ObjectChangeEventStream stream)
    {
        for (int i = 0; i < stream.length; i++)
        {
            ObjectChangeKind type = stream.GetEventType(i);

            if (type == ObjectChangeKind.CreateGameObjectHierarchy)
            {
                stream.GetCreateGameObjectHierarchyEvent(i, out CreateGameObjectHierarchyEventArgs args);
                CheckAndReparentNode(args.instanceId);
            }
        }
    }

    static void OnHierarchyChanged()
    {
        if (Application.isPlaying) return;

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name != NODE_GRAPH_SCENE_NAME) return;

        GameObject[] rootObjects = activeScene.GetRootGameObjects();
        
        foreach (GameObject rootObj in rootObjects)
        {
            NodeStateMachine nodeSM = rootObj.GetComponent<NodeStateMachine>();
            
            if (nodeSM != null)
            {
                Transform nodesWrapper = FindNodesWrapper();
                
                if (nodesWrapper != null && rootObj.transform.parent != nodesWrapper)
                {
                    Undo.SetTransformParent(rootObj.transform, nodesWrapper, "Auto-parent Node to Nodes wrapper");
                    Debug.Log($"Auto-parented '{rootObj.name}' to Nodes wrapper");
                }
            }
        }
    }

    static void CheckAndReparentNode(int instanceId)
    {
        EditorApplication.delayCall += () =>
        {
            if (Application.isPlaying) return;
            
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != NODE_GRAPH_SCENE_NAME) return;

            GameObject obj = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            
            if (obj == null) return;

            NodeStateMachine nodeSM = obj.GetComponent<NodeStateMachine>();
            
            if (nodeSM != null)
            {
                Transform nodesWrapper = FindNodesWrapper();
                
                if (nodesWrapper != null && obj.transform.parent != nodesWrapper)
                {
                    Undo.SetTransformParent(obj.transform, nodesWrapper, "Auto-parent Node to Nodes wrapper");
                    Debug.Log($"Auto-parented '{obj.name}' to Nodes wrapper");
                }
            }
        };
    }

    static Transform FindNodesWrapper()
    {
        NodeGraphController controller = Object.FindFirstObjectByType<NodeGraphController>();
        
        if (controller == null) return null;

        Transform nodesWrapper = controller.transform.Find(NODES_WRAPPER_NAME);
        
        if (nodesWrapper == null)
        {
            Debug.LogWarning($"'{NODES_WRAPPER_NAME}' wrapper not found under Node Graph Controller! Nodes will not auto-parent correctly.");
        }
        
        return nodesWrapper;
    }
}
