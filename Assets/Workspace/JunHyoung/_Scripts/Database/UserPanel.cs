using Firebase.Database;
using Firebase.Extensions;
using LoginSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : MonoBehaviour
{
    [SerializeField] TMP_Text nickNameText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Button editInfo;
    [SerializeField] EditPanel editPanel;

    private bool isActive = false;

    private void Start()
    {
        editInfo.onClick.AddListener(ActivePanel);
    }

    private void OnEnable()
    {
        FirebaseManager.DB.GetReference(FirebaseManager.PATH)
         .Child(FirebaseManager.Auth.CurrentUser.UserId)
         .ValueChanged += UpdateInfo;
    }

    private void OnDisable()
    {
        FirebaseManager.DB.GetReference(FirebaseManager.PATH)
          .Child(FirebaseManager.Auth.CurrentUser.UserId)
          .ValueChanged -= UpdateInfo;
    }

    private void ActivePanel()
    {
         //isActive = !isActive;
        editPanel.gameObject.SetActive(true);
    }

    private void UpdateInfo( object sendor, ValueChangedEventArgs args )
    {
        FirebaseManager.DB
          .GetReference(FirebaseManager.PATH).Child(FirebaseManager.Auth.CurrentUser.UserId)
          .GetValueAsync().ContinueWithOnMainThread(task =>
          {
              if ( task.IsFaulted )
              {
                  Debug.LogError($"DB GetValueAsync Faulted : {task.Exception}");
                  return;
              }
              if ( task.IsCanceled )
              {
                  Debug.LogError($"DB SetValueAsync Canceled");
                  return;
              }

              DataSnapshot snapshot = task.Result;
              if ( snapshot.Exists )
              {
                  string json = snapshot.GetRawJsonValue();
                  UserData userData = JsonUtility.FromJson<UserData>(json);
                  nickNameText.text = userData.Name;
                  scoreText.text = userData.score.ToString();
                  return;
              }
          });
    }

}
