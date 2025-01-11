using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private bool isUsed;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && !isUsed)
        {
            UIManager.Instance.UIList[2].gameObject.SetActive(true);
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Player" && !isUsed)
        {
            UIManager.Instance.UIList[2].gameObject.SetActive(false);
        }
    }
}
