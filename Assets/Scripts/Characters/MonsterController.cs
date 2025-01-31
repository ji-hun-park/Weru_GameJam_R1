using System;
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
    public const float maxDistance = 200f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private Vector3 dir;

    private Coroutine atkCo;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (GameManager.Instance.isEventAnim && myType == Enemy.Anim)
        {
            StartCoroutine(PlayStartAnimation());
        }
        
        initialPosition = transform.position;
    }
    
    void FixedUpdate()
    {
        if (Vector3.Distance(initialPosition, transform.position) > maxDistance)
        {
            StopCoroutine(ProjectileAttack());
            //rb.MovePosition(transform.position + (initialPosition - transform.position) * 300f * Time.deltaTime);
            rb.linearVelocity = (initialPosition - transform.position).normalized * 200f;
        }
    }

    private IEnumerator PlayStartAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.UIList[0].gameObject.SetActive(true);
        // 플레이어를 바라보는 방향 계산
        dir = (GameManager.Instance.player.transform.position - transform.position).normalized;
        float timer = 0;
        
        while (timer < 0.75f)
        {
            timer += Time.deltaTime;
            //rb.MovePosition(transform.position + dir * 30f * Time.deltaTime);
            rb.linearVelocity = dir * 75f;
            yield return null;
        }
        
        SpawnVenom();
        
        yield return new WaitForSeconds(1.5f);
        
        rb.useGravity = false;
        
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            //rb.MovePosition(transform.position + Vector3.up * 60f * Time.deltaTime);
            rb.linearVelocity = Vector3.up * 100f;
            yield return null;
        }
        
        GameManager.Instance.playerHP += 20f;
        
        if (UIManager.Instance.UIList[0] != null)
        {
            UIManager.Instance.UIList[0].GetComponent<IngameUI>().StartTimer();
        }
        GameManager.Instance.isEventAnim = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && myType == Enemy.Normal)
        {
            if (atkCo == null) atkCo = StartCoroutine(ProjectileAttack());
        }
    }

    private IEnumerator ProjectileAttack()
    {
        // 플레이어를 바라보는 방향 계산
        targetPosition = GameManager.Instance.player.transform.position;
        targetPosition.y = transform.position.y;
        dir = (targetPosition - transform.position).normalized;
        transform.LookAt(targetPosition);
        float timer = 0;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            //rb.MovePosition(transform.position + dir * 300f * Time.deltaTime);
            rb.linearVelocity = dir * 30f;
            yield return null;
        }
        SpawnVenom();
        yield return new WaitForSeconds(1f);
        atkCo = null;
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
