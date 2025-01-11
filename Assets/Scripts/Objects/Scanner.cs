using UnityEngine;

public class Scanner : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            UIManager.Instance.UIList[2].gameObject.SetActive(true);
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            UIManager.Instance.UIList[2].gameObject.SetActive(false);
        }
    }
}
