using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class NodeAutoParenting
{
    const string NODE_GRAPH_SCENE_NAME = "Nodes Graph";

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
                NodeGraphController controller = Object.FindFirstObjectByType<NodeGraphController>();
                
                if (controller != null && rootObj.transform.parent != controller.transform)
                {
                    Undo.SetTransformParent(rootObj.transform, controller.transform, "Auto-parent Node to Controller");
                    Debug.Log($"Auto-parented '{rootObj.name}' to Node Graph Controller");
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
                NodeGraphController controller = Object.FindFirstObjectByType<NodeGraphController>();
                
                if (controller != null && obj.transform.parent != controller.transform)
                {
                    Undo.SetTransformParent(obj.transform, controller.transform, "Auto-parent Node to Controller");
                    Debug.Log($"Auto-parented '{obj.name}' to Node Graph Controller");
                }
            }
        };
    }
}
