using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ChatUI : MonoBehaviour
{
    //public TMP_Text chatText;
    public TextMeshProUGUI chatText;
    public UnityEvent onSkipKeyPressed;
    public string colorHex = ColorUtility.ToHtmlStringRGB(Color.red);
    private string originalText;
    public bool isCorrect = false;
    public bool isIncorrect = false;
    public bool OneShot = true;
    
    private void Start()
    {
        OneShot = true;
        InitKeyWord();
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitKeyWord();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)||Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Space))
        {
            onSkipKeyPressed?.Invoke();
        }

        if (APIManager.Instance.isCatch && APIManager.Instance.apiResponse != "" && OneShot)
        {
            CheckAnswer();
            AnswerChange();
            OneShot = false;
        }
    }

    public void InitKeyWord()
    {
        originalText = "키워드 불일치 시 죽게됩니다.\r\n캔버스에 그림 그리고 저장 후 전송하세요!\r\n단축키 : 뒤로가기(ESC), 저장(S), 불러오기(L), 전송(T), 상호작용(F)\r\n되돌리기(Z), 되돌리기 취소(X), 대화는 좌클릭, 스페이스로도 스킵 가능\r\n영역 선택 후 제거(Delete), 그림 초기화(N)\r\n\r\n키워드 : ?";
        chatText.text = originalText;
    }

    public void KeyWordChange(string message)
    {
        string Message = message.Replace(GameManager.Instance.keyWord, $"<color=#{colorHex}>{GameManager.Instance.keyWord}</color>");
        chatText.text = Message;
    }

    private void AnswerChange()
    {
        KeyWordChange(APIManager.Instance.apiResponse);
    }

    private void CheckAnswer()
    {
        // 특정 단어가 텍스트에 포함되어 있는지 확인
        //if (Regex.IsMatch(LLMAPIManager.Instance.apiResponse, $@"(^|[^가-힣]){Regex.Escape(GameManager.Instance.keyWord)}([^가-힣]|$)"))
        if (Regex.IsMatch(APIManager.Instance.apiResponse, GameManager.Instance.keyWord))
        {
            // 단어를 리치 텍스트 태그로 감쌈
            /*string coloredText = Regex.Replace(
                originalText,
                $@"\b{Regex.Escape(wordToColor)}\b",
                $"<color=#{colorHex}>{wordToColor}</color>"
            );*/
            
            isCorrect = true;
        }
        else
        {
            // 키워드가 없는 경우 다른 기능 수행
            isIncorrect = true;
        }
    }

    public void OnClickCloseButton()
    {
        Time.timeScale = 1;
        if (APIManager.Instance.isCatch)
        {
            if (isCorrect)
            {
                GameManager.Instance.clearFlag = true;
            }
            else if (isIncorrect)
            {
                GameManager.Instance.failFlag = true;
            }
        }
        gameObject.SetActive(false);
    }
}
