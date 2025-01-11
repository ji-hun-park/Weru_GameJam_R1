using System;
using System.Collections;
using UnityEngine;

public class Venom : MonoBehaviour
{
    private float moveSpeed = 200;

    // Update is called once per frame
    void Update()
    {
        GuidedMissile();
    }

    private void OnEnable()
    {
        StartCoroutine(LifeCycle());
    }

    private void GuidedMissile()
    {
        transform.Rotate(GameManager.Instance.player.transform.position);
        transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.player.transform.position, moveSpeed * Time.deltaTime);
    }

    private IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(2f);
        DestroyVenom();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.player.gameObject.GetComponent<PlayerController>().Damaged();
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
