using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnClickStartButton()
    {
        GameManager.Instance.playerHP = 100f;
        GameManager.Instance.playerMP = 100f;
        SceneManager.LoadScene("IngameScene");
    }
    
    public void OnClickEndButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
