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
        GameManager.Instance.clearFlag = false;
        GameManager.Instance.failFlag = false;
        GameManager.Instance.isIngame = false;
        GameManager.Instance.isEventAnim = false;
        GameManager.Instance.isRevers = false;
        APIManager.Instance.isCatch = false;
        APIManager.Instance.apiResponse = null;
        APIManager.Instance.messageList.Clear();
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
