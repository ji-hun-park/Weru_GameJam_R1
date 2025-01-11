using System;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private bool isUsed;

    private void OnEnable()
    {
        isUsed = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && !isUsed)
        {
            UIManager.Instance.UIList[2].gameObject.SetActive(true);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Player" && !isUsed)
        {
            if (Input.GetKey(KeyCode.F))
            {
                Time.timeScale = 0;
                UIManager.Instance.UIList[2].gameObject.SetActive(false);
                UIManager.Instance.UIList[6].gameObject.SetActive(true);
                isUsed = true;
            }
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
