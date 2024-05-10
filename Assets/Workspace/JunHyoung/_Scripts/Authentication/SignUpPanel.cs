using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoginSystem
{
    public class SignUpPanel : MonoBehaviour
    {
        [SerializeField] LoginManager panelController;

        [SerializeField] TMP_InputField emailInputField;
        [SerializeField] TMP_InputField passInputField;
        [SerializeField] TMP_InputField confirmInputField;

        [SerializeField] TMP_InputField nameInputField;

        [SerializeField] Button cancelButton;
        [SerializeField] Button signUpButton;

        private void Awake()
        {
            cancelButton.onClick.AddListener(Cancel);
            signUpButton.onClick.AddListener(SignUp);
        }

        public void SignUp()
        {
            SetInteractable(false);
            string id = emailInputField.text;
            string pass = passInputField.text;
            string confirm = confirmInputField.text;
            string name = nameInputField.text;

            // 비밀번호 확인
            if ( pass != confirm )
            {
                panelController.ShowInfo("Password doesn't matched");
                SetInteractable(true);
                return;
            }

            //닉네임 확인
            if ( name == null )
            {
                panelController.ShowInfo("NickName Field is Empty!");
                SetInteractable(true);
                return;
            }
            FirebaseManager.Auth.CreateUserWithEmailAndPasswordAsync(id, pass).ContinueWithOnMainThread(task =>
            {
                if ( task.IsCanceled )
                {
                    panelController.ShowInfo("CreateUserWithEmailAndPasswordAsync was canceled.");
                    SetInteractable(true);

                    return;
                }
                if ( task.IsFaulted )
                {
                    panelController.ShowInfo("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    SetInteractable(true);
                    return;
                }
                // Firebase user has been created.
                Firebase.Auth.AuthResult result = task.Result;
                panelController.ShowInfo($"Firebase user created successfully:" +
                    $" {result.User.DisplayName} ({result.User.UserId})");
                NameApply(name);
                VCamController.Instance.RotateVCam();
                panelController.SignUpToLogin(id, pass);
                panelController.SetActivePanel(LoginManager.Panel.Login);
                SetInteractable(true);
            });
        }


        private void NameApply( string name )
        {
            //UserProfile Update
            UserProfile profile = new UserProfile();
            profile.DisplayName = name;
            FirebaseManager.Auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
            {
                if ( task.IsCanceled )
                {
                    //panelController.ShowInfo("UpdateUserProfileAsync Canceled");
                    Debug.Log("UpdateUserProfileAsync Canceled");
                    return;
                }
                else if ( task.IsFaulted )
                {
                    //panelController.ShowInfo($"UpdateUserProfileAsync failed : {task.Exception.Message}");
                    Debug.LogException(task.Exception);
                    return;
                }
                //panelController.ShowInfo("UpdateUserProfileAsync Success!");
                Debug.Log("UpdateUserProfileAsync Success!");
            });

            //Database Update
            UserData userData = new UserData(name);
            string json = JsonUtility.ToJson(userData);
            FirebaseManager.DB
                .GetReference("UserData")
                .Child(FirebaseManager.Auth.CurrentUser.UserId)
                .SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
                {
                    if ( task.IsFaulted )
                    {
                        Debug.Log($"DB SetValueAsync Faulted : {task.Exception}");
                        return;
                    }
                    if ( task.IsCanceled )
                    {
                        Debug.Log("DB SetValueAsync Canceled");
                        return;
                    }
                    Debug.Log("All Success");
                    panelController.ShowInfo("SetValueAsync Success!");
                });
        }


        public void Cancel()
        {
            VCamController.Instance.RotateVCam();
            panelController.SetActivePanel(LoginManager.Panel.Login);
        }

        private void SetInteractable( bool interactable )
        {
            emailInputField.interactable = interactable;
            passInputField.interactable = interactable;
            confirmInputField.interactable = interactable;
            cancelButton.interactable = interactable;
            signUpButton.interactable = interactable;
        }
    }

}
