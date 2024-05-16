using TMPro;
using UnityEngine;

public class ChatEntry : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text chatText;

    public void Set( ChatData chatData )
    {
        nameText.text = chatData.name;
        chatText.text = chatData.message;
        nameText.color = chatData.nameColor;
        chatText.color = chatData.messageColor;
    }

    public void Set( string name, string chat )
    {
        nameText.text = name;
        chatText.text = chat;
    }

    public void Set( string name, string chat, Color nameColor, Color textColor )
    {
        nameText.text = name;
        nameText.color = nameColor;
        chatText.text = chat;
        chatText.color = textColor;
    }
}
