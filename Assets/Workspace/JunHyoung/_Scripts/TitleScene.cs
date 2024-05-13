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
        //Debug.Log(PhotonNetwork.NetworkClientState);
        if(PhotonNetwork.NetworkClientState == ClientState.Joined ) //해당 상태가 맞는지 테스트하면서 작업할것.
        {
            ActiveRoom();
            return;
        }

        if( PhotonNetwork.NetworkClientState != ClientState.Disconnected )
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

    private void ActiveRoom()
    {
        titleCanvas.gameObject.SetActive(false);
        loginCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(true);
        lobbyCanvas.SetActivePanel(LobbyManager.Panel.Room);
    }


}
