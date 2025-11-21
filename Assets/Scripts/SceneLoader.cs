using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    const string NODES_GRAPH_SCENE = "Nodes Graph";
    const string LIST_SCENE = "List";

    public void LoadNodesGraph()
    {
        LoadScene(NODES_GRAPH_SCENE);
    }

    public void LoadList()
    {
        LoadScene(LIST_SCENE);
    }

    void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' not found in build settings!");
        }
    }

    public static void LoadSceneStatic(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' not found in build settings!");
        }
    }
}
