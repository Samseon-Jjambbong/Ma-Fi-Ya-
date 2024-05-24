using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


public class SettingPanel : PopUpUI
{
    [Header("Scene Name")]
    [SerializeField] string TitleSceneName;

    [Header("Components")]
    [SerializeField] Button closeButton;
    [SerializeField] Button exitGameButton;
    [SerializeField] Button returnLobbyButton;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;


    protected override void Awake()
    {
        closeButton.onClick.AddListener(Close);
        exitGameButton.onClick.AddListener(ExitGame);
        returnLobbyButton.onClick.AddListener(ReturnLobby);
        bgmSlider.onValueChanged.AddListener(ChangeBGMVol);
        sfxSlider.onValueChanged.AddListener(ChangeSFXVol);

        bgmSlider.value = Manager.Sound.BGMVolme;
        sfxSlider.value = Manager.Sound.SFXVolme;

        if(UnitySceneManager.GetActiveScene().name== TitleSceneName)
            returnLobbyButton.gameObject.SetActive(false);
    }

    void ChangeBGMVol(float val)
    {
        Manager.Sound.BGMVolme = val;
    }

    void ChangeSFXVol(float val)
    {
        Manager.Sound.SFXVolme = val;
    }

    public void Open()
    {
        Manager.UI.ShowPopUpUI(this);
    }

    void ReturnLobby()
    {
        PhotonNetwork.LeaveRoom();
        Manager.Scene.LoadScene(TitleSceneName);
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
