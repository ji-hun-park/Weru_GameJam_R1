using UnityEngine;

public class PowerJump : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Super Jumping!");
            GameManager.Instance.superJumping = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.superJumping = false;
        }
    }
}
