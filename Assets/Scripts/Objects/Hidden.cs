using UnityEngine;

public class Hidden : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hidden!");
            GameManager.Instance.playerHP = 100f;
            UIManager.Instance.RunPopupCoroutine("키워드는 " + GameManager.Instance.keyWord);
        }
    }
}
