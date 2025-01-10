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
    
    Vector3 rotationOffset = new Vector3(0f, 0f, 90f);
    
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
        Vector3 dir = (GameManager.Instance.player.transform.position - transform.position).normalized;
        float timer = 0;
        
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            rb.MovePosition(transform.position + dir * 40f * Time.deltaTime);
            yield return null;
        }

        SpawnVenom();
        //this.gameObject.SetActive(false);
        //GameManager.Instance.isEventAnim = false;
    }

    private GameObject SpawnVenom()
    {
        // 현재 회전 값에 90도 오프셋 추가
        Quaternion spawnRotation = Quaternion.Euler(transform.eulerAngles + rotationOffset);
        GameObject venom = Instantiate(venomPrefab, transform.position, spawnRotation);
        return venom;
    }
}
