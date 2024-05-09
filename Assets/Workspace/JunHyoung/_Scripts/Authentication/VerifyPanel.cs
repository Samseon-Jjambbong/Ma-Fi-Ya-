using Firebase.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VerifyPanel : MonoBehaviour
{
    [SerializeField] PanelController panelController;

    [SerializeField] Button logoutButton;
    [SerializeField] Button sendButton;
    [SerializeField] TMP_Text sendButtonText;

    [SerializeField] int sendMailCooltime;

    private void Awake()
    {
        logoutButton.onClick.AddListener(Logout);
        sendButton.onClick.AddListener(SendVerifyMail);
    }

    private void OnEnable()
    {
        if ( FirebaseManager.Auth == null )
            return;

        verifyCheckRoutine = StartCoroutine(VerifyCheckRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    Coroutine verifyCheckRoutine;
    IEnumerator VerifyCheckRoutine()
    {
        while ( true )
        {
            yield return new WaitForSeconds(3f);

            //현재 유저의 정보를 리로딩, 안하면 적용 안되어있음
            FirebaseManager.Auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(task =>
            {
                if ( task.IsCanceled )
                {
                    panelController.ShowInfo("ReloadAsync Canceled");
                    return;
                }
                else if ( task.IsFaulted )
                {
                    panelController.ShowInfo($"ReloadAsync failed : {task.Exception.Message}");
                }

                // 리로딩 정상적으로 완료
                if ( FirebaseManager.Auth.CurrentUser.IsEmailVerified )
                {
                    panelController.SetActivePanel(PanelController.Panel.Main);
                }
            });
        }
    }

    private void Logout()
    {
        FirebaseManager.Auth.SignOut();
        VCamController.Instance.RotateVCam(VCamController.VCam.Login, -1);
        panelController.SetActivePanel(PanelController.Panel.Login);
    }

    private void SendVerifyMail()
    {
        FirebaseManager.Auth.CurrentUser.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            if ( task.IsCanceled )
            {
                panelController.ShowInfo("SendEmailVerificationAsync canceled");
                return;
            }
            else if ( task.IsFaulted )
            {
                panelController.ShowInfo("SendEmailVerificationAsync faulted");
                return;
            }
        });
    }
}
