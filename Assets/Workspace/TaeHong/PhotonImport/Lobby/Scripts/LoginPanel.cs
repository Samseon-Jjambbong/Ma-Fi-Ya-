using Photon.Pun;
using TMPro;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField idInputField;

    private void Start()
    {
        idInputField.text = $"Player {Random.Range(1000, 10000)}";
    }

    public void Login()
    {
        if (idInputField.text == "")
        {
            Debug.LogError("Empty nickname : Please enter a nickname");
            return;
        }
        
        PhotonNetwork.LocalPlayer.NickName = idInputField.text;
        PhotonNetwork.ConnectUsingSettings();
    }
}
