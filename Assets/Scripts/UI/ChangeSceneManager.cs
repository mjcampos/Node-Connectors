using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneManager : MonoBehaviour
{
    const string NodesGraphScene = "Nodes Graph";
    const string ListScene = "List";

    public void ChangeScene()
    {
        if (SceneManager.GetActiveScene().name == NodesGraphScene)
        {
            SceneManager.LoadScene(ListScene);
        }
        else
        {
            SceneManager.LoadScene(NodesGraphScene);
        }
    }
}
