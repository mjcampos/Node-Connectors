using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class NodeGraphSceneValidator
{
    const string NODE_GRAPH_SCENE_NAME = "Nodes Graph";
    const string CONTROLLER_NAME = "Node Graph Controller";

    static NodeGraphSceneValidator()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        if (scene.name == NODE_GRAPH_SCENE_NAME)
        {
            ValidateNodeGraphScene(scene);
        }
    }

    static void ValidateNodeGraphScene(Scene scene)
    {
        NodeGraphController controller = Object.FindFirstObjectByType<NodeGraphController>();

        if (controller == null)
        {
            if (EditorUtility.DisplayDialog(
                    "Missing Node Graph Controller",
                    $"The '{scene.name}' scene requires a '{CONTROLLER_NAME}' GameObject.\n\nWould you like to create it automatically?",
                    "Create",
                    "Cancel"))
            {
                CreateNodeGraphController();
            }
            else
            {
                Debug.LogWarning($"Scene '{scene.name}' is missing required '{CONTROLLER_NAME}' GameObject!");
            }
        }
    }

    static void CreateNodeGraphController()
    {
        GameObject controllerObj = new GameObject(CONTROLLER_NAME);
        controllerObj.AddComponent<NodeGraphController>();
        controllerObj.AddComponent<InputReader>();
        
        Undo.RegisterCreatedObjectUndo(controllerObj, "Create Node Graph Controller");
        Selection.activeGameObject = controllerObj;
        
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        
        Debug.Log($"Created '{CONTROLLER_NAME}' GameObject in scene.");
    }
}
