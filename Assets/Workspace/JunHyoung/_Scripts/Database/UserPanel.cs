using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : MonoBehaviour
{
    [SerializeField] TMP_Text nickNameText;
    [SerializeField] TMP_InputField nickNameInputField;
    [SerializeField] Button buttonGetDB;
    [SerializeField] Button buttonSetDB;
    

    void Start()
    {
        buttonGetDB.onClick.AddListener(ButtonGetDB);
        

        UserData data = new UserData("ParkJunHyoung");

        string json = JsonUtility.ToJson(data);
        //SetRawDB("UserData", "Park", json);

        // GetDB("UserData", "Park");
    }

    public void ButtonGetDB()
    {
        GetDB("UserData", "Park");
    }


    UserData userData;
    private void GetDB( string path, string child )
    {
        FirebaseManager.DB
            .GetReference(path).Child(child)
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if ( task.IsFaulted )
                {
                    return;
                }
                if ( task.IsCanceled )
                {

                    return;
                }

                DataSnapshot snapshot = task.Result;
                if ( snapshot.Exists )
                {
                    string json = snapshot.GetRawJsonValue();
                    Debug.Log(json);
                    userData = JsonUtility.FromJson<UserData>(json);

                    Debug.Log($"{userData.nickName}");
                    return;
                }
                else
                {
                    userData = new UserData();
                }
            });
    }

    public void ChangeNickName()
    {
        string nickName = nickNameInputField.text;

        FirebaseManager.DB
            .GetReference("UserData")
            .Child("")
            .Child("nickName")
            .SetValueAsync(nickName).ContinueWithOnMainThread(task =>
            {
                if ( task.IsFaulted )
                {
                    return;
                }
                if ( task.IsCanceled )
                {
                    return;
                }
            });
    }


    private void SetRawDB( string path, string child, string value )
    {
        FirebaseManager.DB
            .GetReference(path).Child(child)
            .SetRawJsonValueAsync(value).ContinueWithOnMainThread(task =>
            {
                if ( task.IsFaulted )
                {
                    return;
                }
                if ( task.IsCanceled )
                {
                    return;
                }

                Debug.Log("SetRawJsonValueAsync Done");
                return;
            });
    }
}
