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
        nameText.color = chatData.nameColor;
        chatText.text = chatData.message;
        chatText.color = chatData.messageColor;
    }
    public void SetTextColor( Color textColor )
    { 
        chatText.color = textColor;
    }
}
