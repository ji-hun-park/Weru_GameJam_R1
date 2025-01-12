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
        // 키보드 Enter 버튼으로 전송
        if (TMPInputField.text.Length > 0 && Input.GetKeyDown(KeyCode.Return))
        {
            OnClickSendButton();
        }

        if (APIManager.Instance.isError)
        {
            TMPText.text = "응답 오류! 재전송 하세요!";
            oneShot = true;
        }
        
        if (APIManager.Instance.isCatch)
        {
            ChangeField();
        }
    }

    private void OnEnable()
    {
        oneShot = true;
        TMPText.text = "아래에 질문을 입력하세요!";
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
        rectTransform.gameObject.SetActive(false);
        Time.timeScale = 1;
        APIManager.Instance.isCatch = false;
        gameObject.SetActive(false);
    }
}
