using LoginSystem;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : BaseScene
{
    [SerializeField] TitleCanvas titleCanvas;
    [SerializeField] LoginManager loginCanvas;
    [SerializeField] LobbyManager lobbyCanvas;

    [SerializeField] AudioClip mainBGM;
    [SerializeField] AudioClip roomBGM;

    private void Awake()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.Leaving || PhotonNetwork.NetworkClientState == ClientState.Joined)
        {
            ActiveLobby();
            return;
        }

        InitTitleScene();
    }

    private void InitTitleScene()
    {
        titleCanvas.gameObject.SetActive(true);
        loginCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(false);
        Manager.Sound.PlayBGM(mainBGM);
    }

    private void ActiveLobby()
    {
        VCamController.Instance.SetVCam(VCamController.VCam.Lobby);
        titleCanvas.gameObject.SetActive(false);
        loginCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(true);
    }

    public override IEnumerator LoadingRoutine()
    {
        yield return null;
    }
}
