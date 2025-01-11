using System;
using TMPro;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    public TMP_Text textArea;

    private void Update()
    {
        textArea.text = UIManager.Instance.chatMessage;
    }
}
