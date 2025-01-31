using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴 적용
    public static GameManager Instance;

    [Header("Stats")]
    public float playerHP;
    public float playerMP;
    public float leftTime;

    [Header("Settings")]
    [SerializeField] private string KeyWord;

    public string keyWord
    {
        get => KeyWord; set => KeyWord = value;
    }
    
    public event Action OnFlagTrue; // 이벤트 선언
    public UnityEvent onFlagTrue; // UnityEvent 선언
    
    [Header("GameObjects")]
    public GameObject player;
    public GameObject tmp;
    public GameObject animMon;

    [Header("Flags")]
    public bool isEventAnim;
    public bool isIngame;
    public bool isRevers;
    public bool isRevers2;
    public bool superJumping;
    [SerializeField] private bool ClearFlag;
    public bool clearFlag
    {
        get => ClearFlag;
        set
        {
            if (!ClearFlag && value) // 값이 false에서 true로 변경될 때만 실행
            {
                ClearFlag = value;
                OnFlagTrue?.Invoke(); // 이벤트 발생
            }
            else
            {
                ClearFlag = value;
            }
        }
    }
    [SerializeField] private bool FailFlag;
    public bool failFlag
    {
        get => FailFlag;
        set
        {
            if (!FailFlag && value) // 값이 false에서 true로 변경될 때만 실행
            {
                FailFlag = value;
                onFlagTrue?.Invoke(); // UnityEvent 호출
            }
            else
            {
                FailFlag = value;
            }
        }
    }
    
    [Header("Lists")]
    [SerializeField]
    private List<string> KeyWordList;
    
    // Update is called once per frame
    private void Awake()
    {
        // Instance 존재 유무에 따라 게임 매니저 파괴 여부 정함
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 기존에 존재 안하면 이걸로 대체하고 파괴하지 않기
        }
        else
        {
            Destroy(gameObject); // 기존에 존재하면 자신파괴
        }
        
        playerHP = 100f;
        playerMP = 100f;
        leftTime = 600f;
        
        KeyWordListSetting();
        
        OnFlagTrue += HandleFlagTrue; // 이벤트 구독
    }
    
    private void HandleFlagTrue()
    {
        Debug.Log("GameClear");
        Time.timeScale = 0;
        UIManager.Instance.UIList[7].gameObject.SetActive(true);
        UIManager.Instance.UIList[7].GetComponent<OverUI>().SetText(true);
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
        Debug.Log($"Scene Loaded: {scene.name}");
        if (scene.name == "IngameScene")
        {
            isIngame = true;
            isEventAnim = true;
        }
        InitializeScene(scene);
        SelectKeyWord();
    }

    private void InitializeScene(Scene scene)
    {
        if (isIngame)
        {
            player = FindWithTag("Player");
            animMon = FindWithTag("AnimMonster");
        }
    }

    public GameObject FindWithTag(string tag)
    {
        tmp = GameObject.FindGameObjectWithTag(tag);
        return tmp;
    }
    
    public void GameOver()
    {
        Debug.Log("GameOver!");
        Time.timeScale = 0;
        UIManager.Instance.UIList[7].gameObject.SetActive(true);
        UIManager.Instance.UIList[7].GetComponent<OverUI>().SetText(false);
    }
    
    private void KeyWordListSetting()
    {
        KeyWordList = new List<string>();
        
        KeyWordList.Add("곰");
        KeyWordList.Add("나무");
        KeyWordList.Add("사람");
        KeyWordList.Add("사과");
        KeyWordList.Add("집");
        KeyWordList.Add("로봇");
        KeyWordList.Add("바다");
        KeyWordList.Add("오징어");
        KeyWordList.Add("파");
        KeyWordList.Add("무지개");
        KeyWordList.Add("인형");
        KeyWordList.Add("공");
        KeyWordList.Add("상자");
        KeyWordList.Add("콩");
        KeyWordList.Add("지갑");
        KeyWordList.Add("학교");
        KeyWordList.Add("키보드");
        KeyWordList.Add("쥐");
        KeyWordList.Add("개미");
        KeyWordList.Add("칼");
        KeyWordList.Add("피아노");
        KeyWordList.Add("약");
        KeyWordList.Add("책");
        KeyWordList.Add("가방");
        KeyWordList.Add("모자");
        KeyWordList.Add("커튼");
        KeyWordList.Add("전화");
        KeyWordList.Add("휴지");
        KeyWordList.Add("종이");
        KeyWordList.Add("배");
        KeyWordList.Add("비행기");
        KeyWordList.Add("차");
        KeyWordList.Add("군인");
        KeyWordList.Add("경찰");
        KeyWordList.Add("소방");
        KeyWordList.Add("총");
        KeyWordList.Add("기타");
        KeyWordList.Add("바이올린");
        KeyWordList.Add("마이크");
        KeyWordList.Add("바람");
        KeyWordList.Add("수영");
        KeyWordList.Add("기와");
        KeyWordList.Add("쌀");
        KeyWordList.Add("밀");
        KeyWordList.Add("빵");
        KeyWordList.Add("케이크");
        KeyWordList.Add("닭");
        KeyWordList.Add("고기");
        KeyWordList.Add("인간");
        KeyWordList.Add("기사");
        KeyWordList.Add("활");
        KeyWordList.Add("창");
        KeyWordList.Add("계란");
        KeyWordList.Add("책상");
        KeyWordList.Add("의자");
        KeyWordList.Add("고리");
        KeyWordList.Add("끈");
        KeyWordList.Add("옷걸이");
        KeyWordList.Add("옷");
        KeyWordList.Add("라면");
        KeyWordList.Add("초콜릿");
        KeyWordList.Add("장난감");
        KeyWordList.Add("국기");
        KeyWordList.Add("해");
        KeyWordList.Add("달");
        KeyWordList.Add("오이");
        KeyWordList.Add("메론");
        KeyWordList.Add("수박");
        KeyWordList.Add("딸기");
        KeyWordList.Add("연필");
        KeyWordList.Add("팬");
        KeyWordList.Add("티셔츠");
        KeyWordList.Add("신발");
        KeyWordList.Add("머리");
        KeyWordList.Add("발");
        KeyWordList.Add("손");
        KeyWordList.Add("털");
        KeyWordList.Add("구슬");
        KeyWordList.Add("동전");
        KeyWordList.Add("지폐");
        KeyWordList.Add("풀");
        KeyWordList.Add("해파리");
        KeyWordList.Add("톱니바퀴");
        KeyWordList.Add("강아지");
        KeyWordList.Add("고양이");
        KeyWordList.Add("후추");
        KeyWordList.Add("그릇");
        KeyWordList.Add("접시");
        KeyWordList.Add("가로등");
        KeyWordList.Add("세수");
        KeyWordList.Add("신호등");
        KeyWordList.Add("축제");
        KeyWordList.Add("대화");
        KeyWordList.Add("사자");
        KeyWordList.Add("호랑이");
        KeyWordList.Add("악어");
        KeyWordList.Add("용");
        KeyWordList.Add("뱀");
        KeyWordList.Add("새");
        KeyWordList.Add("백조");
    }

    public void SelectKeyWord()
    {
        keyWord = KeyWordList[Random.Range(0, KeyWordList.Count)];
    }
}
