using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("WorldMap");
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
