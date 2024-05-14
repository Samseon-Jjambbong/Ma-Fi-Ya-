using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatEntry : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text chatText;

    public void Set(string name, string chat)
    {
        nameText.text = name;
        chatText.text = chat;
    }
}
