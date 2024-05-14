using LoginSystem;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    [SerializeField] TitleCanvas titleCanvas;
    [SerializeField] LoginManager loginCanvas;
    [SerializeField] LobbyManager lobbyCanvas; 
    private void Awake()
    {
        if( PhotonNetwork.NetworkClientState == ClientState.Leaving || PhotonNetwork.NetworkClientState == ClientState.Joined )
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
    }

    private void ActiveLobby()
    {
        titleCanvas.gameObject.SetActive(false);
        loginCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(true);
    }

}
