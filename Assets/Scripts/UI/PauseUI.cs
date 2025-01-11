using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public void OnClickMainButton()
    {
        Time.timeScale = 1;
        GameManager.Instance.isIngame = false;
        GameManager.Instance.isEventAnim = false;
        SceneManager.LoadScene("MainMenuScene");
    }
    
    public void OnClickQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
