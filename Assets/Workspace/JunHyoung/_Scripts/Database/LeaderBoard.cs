using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] Transform contents;
    [SerializeField] UserRank prefab;

    [SerializeField] Button activeButton;

    List<UserRank> userRanks = new List<UserRank>(MAXCOUNT);

    const int MAXCOUNT = 10;
    const string ROOTPATH = "UserData";
    //const string DATA = "score";
    const string DATA = "winCount";

    void Start()
    {
        //InitLeaderBoard();
    }

    private void OnEnable()
    {
        FirebaseManager.DB.GetReference(ROOTPATH)
           .OrderByChild(DATA)
           .LimitToFirst(MAXCOUNT)
           .ValueChanged += UpdateLeaderBoard; //변경시 호출될 함수를 ValueChanged에 델리게이트체인을 걸어 사용
    }

    private void OnDisable()
    {
        FirebaseManager.DB.GetReference(ROOTPATH)
           .OrderByChild(DATA)
           .LimitToFirst(MAXCOUNT)
           .ValueChanged -= UpdateLeaderBoard;
    }

    void InitLeaderBoard()
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
                     userRanks.Add(userRank);
                     rank--;
                 }
             });
    }

    private void UpdateLeaderBoard(object sendor, ValueChangedEventArgs args)
    {
        FirebaseManager.DB
             .GetReference(ROOTPATH)
             .OrderByChild(DATA)
             .GetValueAsync()
             .ContinueWithOnMainThread(task =>
             {
                 if ( task.IsFaulted )
                 {
                     Debug.Log($"Update LeaderBoard data Faulted : {task.Exception.Message}");
                     return;
                 }
                 else if ( task.IsCanceled )
                 {
                     Debug.Log("Update LeaderBoard data Canceled");
                     return;
                 }

                 DataSnapshot snapshot = task.Result;

                 foreach ( Transform child in contents )
                 {
                     Destroy(child.gameObject);  //삭제 생성 대신 더 좋은 방안 생각해 개선할것...
                                                 // => 풀링 해둔 뒤 disable 하고 enable 하면서 세팅?
                 }

                 int rank = ( int ) snapshot.ChildrenCount;
                 Debug.Log(rank);
                 foreach ( var item in snapshot.Children )
                 {
                     string json = item.GetRawJsonValue();
                     UserData data = JsonUtility.FromJson<UserData>(json);
                     string name = data.Name;
                     int score = data.winCount;
                     var userRank = Instantiate(prefab, contents);
                     
                     userRank.Set(rank, name, score);
                     rank--;
                 }
             });
    }

}


[SerializeField]

public class ScoreData
{
    public string nickName;
    public int winCount;
}