using System;
using TMPro;
using UnityEngine;

public class NPCUI : MonoBehaviour
{
    public TMP_Text TMPText;
    public TMP_InputField TMPInputField;
    public string userInput;
    public bool oneShot;
    public RectTransform rectTransform;
    public TMP_Text responseText;
    
    private void Update()
    {
        if (APIManager.Instance.isCatch)
        {
            ChangeField();
        }
    }

    private void OnEnable()
    {
        oneShot = true;
    }

    public void OnClickSendButton()
    {
        if (oneShot)
        {
            userInput = TMPInputField.text;
            APIManager.Instance.SendRequestText(userInput);
            TMPText.text = "응답 대기 중...";
            oneShot = false;
        }
        else
        {
            TMPText.text = "기회가 없습니다!";
        }
    }

    public void ChangeField()
    {
        TMPText.text = "응답이 왔습니다!";
        rectTransform.gameObject.SetActive(true);
        responseText.text = APIManager.Instance.apiResponse;
        APIManager.Instance.isCatch = false;
    }

    public void OnClickReturnButton()
    {
        Time.timeScale = 1;
        APIManager.Instance.isCatch = false;
        gameObject.SetActive(false);
    }
}
