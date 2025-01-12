using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverUI : MonoBehaviour
{
    public TMP_Text overText;
    public string overMessage;

    public void SetText(bool isOver)
    {
        overMessage = (isOver) ? "Game Clear!!!" : "Game Over!";
        overText.color = (isOver) ? Color.blue : Color.red;
        overText.text = overMessage;
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
