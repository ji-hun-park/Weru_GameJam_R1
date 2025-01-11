using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public void ONClickScrollButton()
    {
        UIManager.Instance.UIList[8].gameObject.SetActive(false);
        UIManager.Instance.UIList[4].gameObject.SetActive(true);
    }
    
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
