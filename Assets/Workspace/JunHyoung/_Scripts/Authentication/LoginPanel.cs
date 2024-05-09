using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] PanelController panelController;

    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passInputField;

    [SerializeField] Button signUpButton;
    [SerializeField] Button loginButton;
    [SerializeField] Button resetPasswordButton;

    [SerializeField] bool isDebug; // debug flag for skip email Varify

    private void Awake()
    {
        signUpButton.onClick.AddListener(SignUp);
        loginButton.onClick.AddListener(Login);
        resetPasswordButton.onClick.AddListener(ResetPassword);
    }

    public void SignUp()
    {
        VCamController.Instance.RotateVCam(-1);
        panelController.SetActivePanel(PanelController.Panel.SignUp);
    }

    private void ResetPassword()
    {
        VCamController.Instance.RotateVCam(-1);
        panelController.SetActivePanel(PanelController.Panel.Reset);
    }

    public void Login()
    {
        SetInteractable(false);
        string id = emailInputField.text;
        string pass = passInputField.text;

        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(id, pass).ContinueWithOnMainThread(task =>
        {
            if ( task.IsCanceled )
            {
                panelController.ShowInfo("SignInWithEmailAndPasswordAsync was canceled.");
                SetInteractable(true);
                return;
            }
            if ( task.IsFaulted )
            {
                panelController.ShowInfo($"SignInWithEmailAndPasswordAsync encountered an error: {task.Exception.Message}");
                SetInteractable(true);
                return;
            }

            if ( isDebug )
            {
                panelController.SetActivePanel(PanelController.Panel.Main);
                SetInteractable(true);
            }
            else
            {
                panelController.SetActivePanel(PanelController.Panel.Verify);
                SetInteractable(true);
            }
            //Firebase.Auth.AuthResult result = task.Result;
            //panelController.ShowInfo($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
        });
    }

    public void FromSignUpPanel(string id, string pass )
    {
        emailInputField.text = id;
        passInputField.text = pass;
    }

    private void SetInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        passInputField.interactable = interactable;
        signUpButton.interactable = interactable;
        loginButton.interactable = interactable;
        resetPasswordButton.interactable = interactable;
    }
}
