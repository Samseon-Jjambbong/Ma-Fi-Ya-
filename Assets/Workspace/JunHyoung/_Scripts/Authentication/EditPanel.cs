using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoginSystem
{
    public class EditPanel : MonoBehaviour
    {
        [SerializeField] LoginManager panelController;

        [SerializeField] TMP_InputField nameInputField;
        [SerializeField] TMP_InputField passInputField;
        [SerializeField] TMP_InputField confirmInputField;

        [SerializeField] Button nameApplyButton;
        [SerializeField] Button passApplyButton;
        [SerializeField] Button backButton;
        [SerializeField] Button deleteButton;

        private void Awake()
        {
            nameApplyButton.onClick.AddListener(NameApply);
            passApplyButton.onClick.AddListener(PassApply);
            backButton.onClick.AddListener(Back);
            deleteButton.onClick.AddListener(Delete);
        }

        private void OnDisable()
        {
            nameInputField.text = string.Empty;
            passInputField.text = string.Empty;
            confirmInputField.text = string.Empty;
        }

        private void NameApply()
        {
            SetInteractable(false);

            string name = nameInputField.text;
            if ( FirebaseManager.UpdateName(name) )
            {
                panelController.ShowInfo("Update NickName Success!");
                SetInteractable(true);
            }
            else
            {
                panelController.ShowInfo("Update NickName Failed...");
                SetInteractable(true);
            }
        }

        private void PassApply()
        {
            SetInteractable(false);

            if ( passInputField.text != confirmInputField.text )
            {
                panelController.ShowInfo("Password doesn't matched");
                SetInteractable(true);
                return;
            }

            string passWord = passInputField.text;

            FirebaseManager.Auth.CurrentUser.UpdatePasswordAsync(passWord).ContinueWithOnMainThread(task =>
            {
                if ( task.IsCanceled )
                {
                    panelController.ShowInfo("UpdatePasswordAsync Canceled");
                    SetInteractable(true);
                    return;
                }
                else if ( task.IsFaulted )
                {
                    panelController.ShowInfo($"UpdatePasswordAsync failed : {task.Exception.Message}");
                    SetInteractable(true);
                    return;
                }

                SetInteractable(true);
                panelController.ShowInfo("UpdatePasswordAsync Success!");
            });
        }

        private void Back()
        {
            gameObject.SetActive(false);
        }

        private void Delete()
        {
            SetInteractable(false);
            FirebaseManager.Auth.CurrentUser.DeleteAsync().ContinueWithOnMainThread(task =>
            {
                if ( task.IsCanceled )
                {
                    //panelController.ShowInfo("DeleteAsync Canceled");
                    SetInteractable(true);
                    return;
                }
                else if ( task.IsFaulted )
                {
                    panelController.ShowInfo($"DeleteAsync failed : {task.Exception.Message}");
                    SetInteractable(true);
                    return;
                }

                SetInteractable(true);
                //panelController.ShowInfo("DeleteAsync Success!");
            });
        }

        private void SetInteractable( bool interactable )
        {
            nameInputField.interactable = interactable;
            passInputField.interactable = interactable;
            confirmInputField.interactable = interactable;
            nameApplyButton.interactable = interactable;
            passApplyButton.interactable = interactable;
            backButton.interactable = interactable;
            deleteButton.interactable = interactable;
        }
    }

}
