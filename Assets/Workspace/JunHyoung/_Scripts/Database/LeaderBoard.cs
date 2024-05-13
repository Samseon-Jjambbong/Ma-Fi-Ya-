using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] Transform contents;
    [SerializeField] UserRank prefab;

    [SerializeField] Button activeButton;

    const string ROOTPATH = "UserData";
    //const string DATA = "score";
    const string DATA = "winCount";

    void Start()
    {
        GetScoreBoard();
    }

    private void OnEnable()
    {
        FirebaseManager.DB.GetReference(ROOTPATH)
           .OrderByChild(DATA)
           .LimitToFirst(10)
           .ValueChanged += ScoreBoardChanged; //변경시 호출될 함수를 ValueChanged에 델리게이트체인을 걸어 사용
    }

    private void OnDisable()
    {
        FirebaseManager.DB.GetReference(ROOTPATH)
           .OrderByChild(DATA)
           .LimitToFirst(10)
           .ValueChanged -= ScoreBoardChanged;
    }

    void GetScoreBoard()
    {
        FirebaseManager.DB
             .GetReference(ROOTPATH)
             .OrderByChild(DATA)
             .GetValueAsync()
             .ContinueWithOnMainThread(task =>
             {
                 if ( task.IsFaulted )
                 {
                     Debug.Log($"Get LeaderBoard data Faulted : {task.Exception.Message}");
                     return;
                 }
                 else if ( task.IsCanceled )
                 {
                     Debug.Log("Get LeaderBoard data Canceled");
                     return; 
                 }

                 DataSnapshot snapshot = task.Result;
                 int rank = (int)snapshot.ChildrenCount;
                 Debug.Log(rank);
                 foreach ( var item in snapshot.Children )
                 {
                     string json = item.GetRawJsonValue();
                     UserData data = JsonUtility.FromJson<UserData>(json);
                     string name = data.Name;
                     int score = data.winCount;
                     //Debug.Log($"{name} : {score}");
                     var userRank = Instantiate(prefab, contents);
                     userRank.Set(rank, name ,score);
                     rank--;
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