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
        overText.text = overMessage;
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
