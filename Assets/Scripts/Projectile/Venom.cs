using System;
using System.Collections;
using UnityEngine;

public class Venom : MonoBehaviour
{
    private float moveSpeed = 150;
    private Vector3 targetPosition;
    private Vector3 dir;
    
    // Update is called once per frame
    void Update()
    {
        GuidedMissile();
    }

    private void OnEnable()
    {
        dir = GameManager.Instance.player.transform.position - transform.position;
        targetPosition = GameManager.Instance.player.transform.position;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        StartCoroutine(LifeCycle());
    }

    private void GuidedMissile()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition + dir, moveSpeed * Time.deltaTime);
    }

    private IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(1.5f);
        DestroyVenom();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.player.gameObject.GetComponent<PlayerController>().Damaged();
            DestroyVenom();
        }
        
        if (other.CompareTag("Wall") || other.CompareTag("Floor"))
        {
            DestroyVenom();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall") || other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            DestroyVenom();
        }
    }

    private void DestroyVenom()
    {
        Destroy(gameObject);
    }
}
