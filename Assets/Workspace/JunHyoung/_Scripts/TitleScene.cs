using LoginSystem;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    [SerializeField] TitleCanvas titleCanvas;
    [SerializeField] LoginManager loginCanvas;
    [SerializeField] LobbyManager lobbyCanvas;

    [SerializeField] AudioClip mainBGM;
    [SerializeField] AudioClip roomBGM;

    [SerializeField] AudioClip[] buttonSFXs;
    private void Awake()
    {
        if ( PhotonNetwork.NetworkClientState == ClientState.Leaving || PhotonNetwork.NetworkClientState == ClientState.Joined )
        {
            ActiveLobby();
            return;
        }

        InitTitleScene();
        BindAllButtons();
    }

    private void InitTitleScene()
    {
        titleCanvas.gameObject.SetActive(true);
        loginCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(false);
    }

    private void ActiveLobby()
    {
        VCamController.Instance.SetVCam(VCamController.VCam.Lobby);
        titleCanvas.gameObject.SetActive(false);
        loginCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(true);
    }


    void BindAllButtons()
    {
        Debug.Log("Start Bind");
        Button[] buttons = FindObjectsOfType<Button>();

        // 각 버튼의 onClick 이벤트에 PlaySFX() 메서드 등록
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayRandomSFX);
        }

        Debug.Log("Finish Bind");
    }

    void PlayRandomSFX()
    {

    }
}
