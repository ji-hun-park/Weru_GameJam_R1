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
    
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKey(KeyCode.F) && !UIManager.Instance.UIList[1].gameObject.activeSelf)
            {
                Time.timeScale = 0;
                UIManager.Instance.UIList[2].gameObject.SetActive(false);
                UIManager.Instance.UIList[1].gameObject.SetActive(true);
            }
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
