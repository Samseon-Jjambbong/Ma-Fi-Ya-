using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    
    [SerializeField] TitleCanvas titleCanvas;
    [SerializeField] LoginManager loginCanvas;
    [SerializeField] GameObject lobbyCanvas; // replace this to LobbyManager when Merged

    private void Awake()
    {
        titleCanvas.gameObject.SetActive(true);
        loginCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(false);
    }

}
