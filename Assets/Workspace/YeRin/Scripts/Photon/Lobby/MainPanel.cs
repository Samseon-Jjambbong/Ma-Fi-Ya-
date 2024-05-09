using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    private void OnEnable()
    {
        createRoomPanel.SetActive(false);
    }

    public void CreateRoomMenu()
    {
        createRoomPanel.SetActive(true);
    }

    public void CreateRoomConfirm()
    {
        // 방 만들기 구현
        string roomName = roomNameInputField.text;
        if (roomName == "")
        {
            roomName = $"Room{Random.Range(1000, 10000)}";
        }

        int maxPlayer = maxPlayerInputField.text == "" ? 8 : int.Parse(maxPlayerInputField.text);
        // Mathf.Clamp를 통해 1명에서 최대 8명 입력으로 제한
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 8);

        // 방 옵션 설정 가능
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayer;

        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void CreateRoomCancel()
    {
        createRoomPanel.SetActive(false);
    }

    public void RandomMatching()
    {
        // PhotonNetwork.JoinRandomRoom(); // 비어있는 방 찾기, 없으면 OnJoinRandomRoomFailed

        string name = $"Room{Random.Range(1000, 10000)}";
        RoomOptions options = new RoomOptions() {MaxPlayers = 8};

        PhotonNetwork.JoinRandomOrCreateRoom(roomName : name, roomOptions : options); // 비어있는 방 찾기, 없으면 방 생성
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public void Logout()
    {
        PhotonNetwork.Disconnect();
    }
}
