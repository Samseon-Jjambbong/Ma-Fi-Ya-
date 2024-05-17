using TMPro;
using UnityEngine;

public class ChatEntry : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text chatText;

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    public void SetChat( ChatData chatData )
    {
        nameText.text = chatData.name;
        chatText.text = chatData.message;
        nameText.color = chatData.nameColor;
        chatText.color = chatData.messageColor;
    }
    public void SetTextColor( Color textColor )
    { 
        chatText.color = textColor;
    }
}
