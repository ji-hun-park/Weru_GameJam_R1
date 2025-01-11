using System;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private bool isUsed;
    public Material usedMaterial;  // 감염 Material
    public Material originalMaterial; // 원래 Material
    private Renderer objectRenderer;

    private void OnEnable()
    {
        InitStat();
    }

    private void InitStat()
    {
        isUsed = false;
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            objectRenderer.material = originalMaterial; // 원래 Material 적용
        }
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
                if (objectRenderer != null)
                {
                    objectRenderer.material = usedMaterial; // 사용됨 Material 적용
                }
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
