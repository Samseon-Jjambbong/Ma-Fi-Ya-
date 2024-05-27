using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] Transform contents;
    [SerializeField] UserRank prefab;

    [SerializeField] GameObject ledaerBoardPanel;
    [SerializeField] Button activeButton;
    private bool isActive=false;

    List<UserRank> userRanks = new List<UserRank>(MAXCOUNT);

    const int MAXCOUNT = 10;
    const string DATA = "score"; //"score"


    void Start()
    {
        //InitLeaderBoard();
        activeButton.onClick.AddListener(ActivePanel);
    }

    private void OnEnable()
    {
        FirebaseManager.DB.GetReference(FirebaseManager.PATH)
           .OrderByChild(DATA)
           .LimitToFirst(MAXCOUNT)
           .ValueChanged += UpdateLeaderBoard; //변경시 호출될 함수를 ValueChanged에 델리게이트체인을 걸어 사용
    }

    private void OnDisable()
    {
        FirebaseManager.DB.GetReference(FirebaseManager.PATH)
           .OrderByChild(DATA)
           .LimitToFirst(MAXCOUNT)
           .ValueChanged -= UpdateLeaderBoard;
    }

    void InitLeaderBoard()
    {
        FirebaseManager.DB
             .GetReference(FirebaseManager.PATH)
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
             .GetReference(FirebaseManager.PATH)
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

                 foreach (Transform child in contents)
                 {
                     Destroy(child.gameObject);
                 }

                 DataSnapshot snapshot = task.Result;

                 int rank = ( int ) snapshot.ChildrenCount;
                 foreach ( var item in snapshot.Children )
                 {
                     string json = item.GetRawJsonValue();
                     UserData data = JsonUtility.FromJson<UserData>(json);
                     string name = data.Name;
                     int score = data.score;
                     var userRank = Instantiate(prefab, contents);
                     
                     userRank.Set(rank, name, score);
                     rank--;
                 }
             });
    }

    private void ActivePanel()
    {
        isActive = !isActive;
        ledaerBoardPanel.SetActive(isActive);
    }
}