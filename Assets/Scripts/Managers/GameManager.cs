using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴 적용
    public static GameManager Instance;
    
    [Header("GameObjects")]
    public Transform player;
    public GameObject npc;
    public GameObject tmp;
    
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

        InitializeScene(scene);
    }

    private void InitializeScene(Scene scene)
    {
        tmp = GameObject.FindGameObjectWithTag("Player");
        if (tmp != null) player = tmp.transform;
        //npc = GameObject.FindGameObjectWithTag("NPC");
    }
}
