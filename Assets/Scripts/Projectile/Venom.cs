using UnityEngine;

public class Venom : MonoBehaviour
{
    private Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GuidedMissile();
    }

    private void GuidedMissile()
    {
        Vector3 dir = (GameManager.Instance.player.transform.position - transform.position).normalized;
        rb.MovePosition(transform.position + dir * 40f * Time.deltaTime);
    }
}
