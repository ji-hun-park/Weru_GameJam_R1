using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public enum Enemy
    {
        Normal,
        Anim
    }
    
    public Enemy myType;
    
    public GameObject venomPrefab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance.isEventAnim && myType == Enemy.Anim)
        {
            PlayStartAnimation();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayStartAnimation()
    {
        //
    }
}
