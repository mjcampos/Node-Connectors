using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneManager : MonoBehaviour
{
    const string NodesGraphScene = "Nodes Graph";
    const string ListScene = "List";

    public void ChangeScene()
    {
        SaveCurrentSceneIfNeeded();

        if (SceneManager.GetActiveScene().name == NodesGraphScene)
        {
            SceneManager.LoadScene(ListScene);
        }
        else
        {
            SceneManager.LoadScene(NodesGraphScene);
        }
    }

    void SaveCurrentSceneIfNeeded()
    {
        if (SceneManager.GetActiveScene().name == NodesGraphScene)
        {
            NodeGraphSaveManager saveManager = FindFirstObjectByType<NodeGraphSaveManager>();
            
            if (saveManager != null)
            {
                saveManager.SaveGame();
                Debug.Log("Saved node graph before scene change.");
            }
            else
            {
                Debug.LogWarning("NodeGraphSaveManager not found in scene!");
            }
        }
    }
}
