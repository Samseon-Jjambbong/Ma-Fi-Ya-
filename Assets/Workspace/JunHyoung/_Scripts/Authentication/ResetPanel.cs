using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResetPanel : MonoBehaviour
{
    [SerializeField] PanelController panelController;

    [SerializeField] TMP_InputField emailInputField;

    [SerializeField] Button sendButton;
    [SerializeField] Button cancelButton;

    private void Awake()
    {
        sendButton.onClick.AddListener(SendResetMail);
        cancelButton.onClick.AddListener(Cancel);
    }

    private void SendResetMail()
    {
        SetInteractable(false);

        string email = emailInputField.text;    
        FirebaseManager.Auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if ( task.IsCanceled ) {
                panelController.ShowInfo("SendPasswordResetEmailAsync is Cancel");
                SetInteractable(true);
                return;
            }else if( task.IsFaulted )
            {
                panelController.ShowInfo($"SendPasswordResetEmailAsync is Fail : {task.Exception.Message}");
                SetInteractable(true);
                return;
            }

            SetInteractable(true);
            panelController.ShowInfo("SendPasswordResetEmailAsync Success!");
            panelController.SetActivePanel(PanelController.Panel.Login);
        });
    }

    private void Cancel()
    {
        VCamController.Instance.RotateVCam(-1);
        panelController.SetActivePanel(PanelController.Panel.Login);
    }

    private void SetInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        sendButton.interactable = interactable;
        cancelButton.interactable = interactable;
    }
}
