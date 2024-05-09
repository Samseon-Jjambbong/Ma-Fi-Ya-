using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    [SerializeField] PanelController panelController;

    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text emailText;
    [SerializeField] TMP_Text idText;

    [SerializeField] Button logoutButton;
    [SerializeField] Button editButton;
    [SerializeField] Button startButton;

    private void Awake()
    {
        logoutButton.onClick.AddListener(Logout);
        editButton.onClick.AddListener(Edit);
        startButton.onClick.AddListener(GameStart);
    }

    private void OnEnable()
    {
        if ( FirebaseManager.Auth == null ) return;

        FirebaseUser user = FirebaseManager.Auth.CurrentUser;

        nameText.text = user.DisplayName;
        emailText.text = user.Email;
        idText.text = user.UserId;
    }

    private void Logout()
    {
        FirebaseManager.Auth.SignOut();
        VCamController.Instance.RotateVCam(VCamController.VCam.Login, -1);
        panelController.SetActivePanel(PanelController.Panel.Login);
    }

    private void Edit()
    {
        VCamController.Instance.RotateVCam(VCamController.VCam.Login, -1);
        panelController.SetActivePanel(PanelController.Panel.Edit);
    }

    private void GameStart()
    {
        //Lobby Canvas 활성화 할 것
    }
}
