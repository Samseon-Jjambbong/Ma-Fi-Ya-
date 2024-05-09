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
            Debug.LogError("Empty nickname : Please input name");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = idInputField.text; // 나 유저의 닉네임
        PhotonNetwork.ConnectUsingSettings();   // 이전 프로젝트에서 설정된 값을 가지고 접속 시도
        
        // 접속은 따로, 네트워크는 반응에 반응하는 형식으로 만들어야 함 (Callback)
        // 클라이언트가 서버한테 요청 : PhotonNetwork.
        // 그에 따른 결과를 Callback으로 서버는 클라이언트에게
    }
}
