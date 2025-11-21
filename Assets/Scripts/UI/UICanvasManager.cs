using UnityEngine;
using UnityEngine.SceneManagement;

public class UICanvasManager : MonoBehaviour
{
    const string NODES_GRAPH_SCENE = "Nodes Graph";
    const string LIST_SCENE = "List";
    
    public void GoToList()
    {
        LoadScene(LIST_SCENE);
    }

    public void GoToNodeGraph()
    {
        LoadScene(NODES_GRAPH_SCENE);
    }

    void LoadScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene {sceneName} not found! Add it to Build Settings.");
        }
    }
}
