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
        //ClientState curState = PhotonNetwork.NetworkClientState;
        //이후 게임 종료 후 씬으로 돌아왔을 때,  Photon 상태에 따라 Lobby - main을 띄워줄지 Room 을 띄워줄지 작업할 것.

        titleCanvas.gameObject.SetActive(true);
        loginCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(false);
    }

}
