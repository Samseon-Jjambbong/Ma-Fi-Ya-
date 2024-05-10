using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetScoreBoard();
    }

    private void OnEnable()
    {
        FirebaseManager.DB.GetReference("SocreBoard")
           .OrderByChild("Score")
           .LimitToFirst(3)
           .ValueChanged += ScoreBoardChanged; //변경시 호출될 함수를 ValueChanged에 델리게이트체인을 걸어 사용
    }

    private void OnDisable()
    {
        FirebaseManager.DB.GetReference("SocreBoard")
           .OrderByChild("Score")
           .LimitToFirst(3)
           .ValueChanged -= ScoreBoardChanged;
    }

    void GetScoreBoard()
    {
        FirebaseManager.DB
             .GetReference("ScoreBoard")
             .OrderByChild("Score")
             .GetValueAsync()
             .ContinueWithOnMainThread(task =>
             {
                 if ( task.IsFaulted )
                 {
                     return;
                 }
                 else if ( task.IsCanceled )
                 { return; }

                 DataSnapshot snapshot = task.Result;
                 foreach ( var item in snapshot.Children )
                 {
                     // string json = item.GetRawJsonValue();
                     // ScoreData data = JsonUtility.FromJson<ScoreData>(json);
                     // data.nickName;

                     string nickName = ( string ) item.Child("nickName").Value;
                     int score = ( int ) item.Child("Score").Value;
                     Debug.Log($"{nickName} : {score}");
                 }
             });
    }

    private void ScoreBoardChanged(object sendor, ValueChangedEventArgs args)
    {
        Debug.Log("DB 변경됨");
    }

}


[SerializeField]

public class ScoreData
{
    public string nickName;
    public int winCount;
}