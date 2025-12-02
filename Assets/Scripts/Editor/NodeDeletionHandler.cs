using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class NodeDeletionHandler
{
    static NodeDeletionHandler()
    {
        ObjectChangeEvents.changesPublished += OnObjectChanges;
    }

    static void OnObjectChanges(ref ObjectChangeEventStream stream)
    {
        for (int i = 0; i < stream.length; i++)
        {
            ObjectChangeKind type = stream.GetEventType(i);

            if (type == ObjectChangeKind.DestroyGameObjectHierarchy)
            {
                stream.GetDestroyGameObjectHierarchyEvent(i, out DestroyGameObjectHierarchyEventArgs args);
                RefreshAllNodeGraphs();
            }
        }
    }

    static void RefreshAllNodeGraphs()
    {
        NodeGraphController[] controllers = Object.FindObjectsByType<NodeGraphController>(FindObjectsSortMode.None);
        
        foreach (NodeGraphController controller in controllers)
        {
            EditorApplication.delayCall += () =>
            {
                if (controller != null)
                    controller.TriggerNodeSettingsAdjuster();
            };
        }
    }
}
