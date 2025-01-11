using System.Collections;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public enum Enemy
    {
        Normal,
        Anim
    }
    
    public Enemy myType;
    public Rigidbody rb;
    public GameObject venomPrefab;
    
    private Vector3 rotationOffset = new Vector3(0f, -90f, 90f);
    private Vector3 dir;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (GameManager.Instance.isEventAnim && myType == Enemy.Anim)
        {
            StartCoroutine(PlayStartAnimation());
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    private IEnumerator PlayStartAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        // 플레이어를 바라보는 방향 계산
        dir = (GameManager.Instance.player.transform.position - transform.position).normalized;
        float timer = 0;
        
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            rb.MovePosition(transform.position + dir * 300f * Time.deltaTime);
            yield return null;
        }

        SpawnVenom();
        
        yield return new WaitForSeconds(1f);
        
        rb.useGravity = false;
        
        while (timer < 3f)
        {
            timer += Time.deltaTime;
            rb.MovePosition(transform.position + Vector3.up * 600f * Time.deltaTime);
            yield return null;
        }
        
        this.gameObject.SetActive(false);
        GameManager.Instance.isEventAnim = false;
    }

    private GameObject SpawnVenom()
    {
        // 투사체 회전 설정
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        // 현재 회전 값에 90도 오프셋 추가
        //Quaternion spawnRotation = Quaternion.Euler(transform.eulerAngles + rotationOffset);
        GameObject venom = Instantiate(venomPrefab, transform.position, lookRotation);
        return venom;
    }
}
