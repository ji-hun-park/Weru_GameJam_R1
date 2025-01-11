using UnityEngine;
using UnityEngine.UI;

public class ScrollUI : MonoBehaviour
{
    public GameObject scrollView;
    public Transform content;
    public Sprite imageSprite;
    public Vector2 imageSize;
    public int fontSize = 30;
    void Awake()
    {
        imageSize = new Vector2(900, 500);
        /*scrollView = UIManager.Instance.FindChildByName(transform, "ScrollView").gameObject;
        if (scrollView == null)
        {
            Debug.LogError("ScrollView를 찾을 수 없습니다!");
        }
        else
        {
            content = scrollView.transform.Find("Viewport/Content");
        }
        if (content == null)
        {
            Debug.LogError("Content를 찾을 수 없습니다!");
        }*/
    }

    void OnEnable()
    {
        if (APIManager.Instance.messageList == null || APIManager.Instance.messageList.Count == 0)
        {
            Debug.LogWarning("메시지 목록이 비어 있습니다.");
        }
        else
        {
            RefreshScrollView();
        }
    }
    
    public void RefreshScrollView()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject); // 기존 요소 삭제
        }

        foreach (string message in APIManager.Instance.messageList)
        {
            CreateImageWithText(message);
        }
    }
    
    public void CreateImageWithText(string textContent)
    {
        if (imageSprite == null)
        {
            Debug.LogError("imageSprite가 할당되지 않았습니다!");
            return;
        }
        
        GameObject newImage = new GameObject("DynamicImage", typeof(Image));
        Image imageComponent = newImage.GetComponent<Image>();
        imageComponent.sprite = imageSprite;
        imageComponent.raycastTarget = false;

        RectTransform imageRect = newImage.GetComponent<RectTransform>();
        imageRect.SetParent(content, false);
        imageRect.sizeDelta = imageSize;

        int childCount = content.childCount - 1; // 새로 추가된 항목을 기준으로 계산
        float spacing = 10f; // 아이템 간 간격
        float newY = -childCount * (imageSize.y + spacing); // 새 위치 계산
        imageRect.anchoredPosition = new Vector2(0, newY); // X는 0, Y는 계산된 값

        GameObject newText = new GameObject("DynamicText", typeof(Text));
        Text textComponent = newText.GetComponent<Text>();

        textComponent.text = textContent;
        textComponent.font = Resources.Load<Font>("Maplestory Bold");
        //textComponent.fontSize = fontSize - 2; // 폰트 크기 조금 축소
        textComponent.color = Color.black;
        textComponent.alignment = TextAnchor.MiddleCenter; // 중앙 정렬
        textComponent.resizeTextForBestFit = true; // 자동 폰트 크기 조정
        textComponent.resizeTextMinSize = 12; // 최소 폰트 크기
        textComponent.resizeTextMaxSize = fontSize;

        RectTransform textRect = newText.GetComponent<RectTransform>();
        textRect.SetParent(newImage.transform, false);
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = new Vector2(20, 20); // 테두리 여백 설정
        textRect.offsetMax = new Vector2(-20, -20);
        textRect.anchoredPosition = Vector2.zero;
    }

    public void OnClickReturnButton()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }    
}
