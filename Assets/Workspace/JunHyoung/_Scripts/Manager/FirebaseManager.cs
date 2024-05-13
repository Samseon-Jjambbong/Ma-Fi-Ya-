using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    private static FirebaseManager instance;
    public static FirebaseManager Instance { get { return instance; } }

    private static FirebaseApp app;
    public static FirebaseApp App { get { return app; } }

    private static FirebaseAuth auth;
    public static FirebaseAuth Auth { get { return auth; } }

    private static FirebaseDatabase db;

    public static FirebaseDatabase DB { get { return db; } }

    private static bool isValid;
    public static bool IsValid { get { return isValid; } }


    /******************************************************
    *                    Init Settings
    ******************************************************/
    #region Init
    private void Awake()
    {
        CreateInstance();
        CheckDependency();
    }

    private void CreateInstance()
    {
        if ( instance == null )
        {
            instance = this;
            profile = new UserProfile();
            userData = new UserData();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void CheckDependency()
    {
        DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if ( dependencyStatus == DependencyStatus.Available )
        {
            // Create and hold a reference to your FirebaseApp,
            // where app is a Firebase.FirebaseApp property of your application class.
            app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseDatabase.DefaultInstance;

            // Set a flag here to indicate whether Firebase is ready to use by your app.
            Debug.Log("Firebase Check and FixDependencies success");
            isValid = true;
        }
        else
        {
            // Firebase Unity SDK is not safe to use here.
            Debug.LogError("Firebase Check and FixDependencies fail");
            isValid = false;

            app = null;
            auth = null;
            db = null;
        }
    }
    #endregion

    /******************************************************
    *                    Public Methods
    ******************************************************/
    private static UserProfile profile;
    private static UserData userData;
    public static UserData UserData { get { return userData; } }

    private const string VALIDFAIL = "Instance is not Valid";
    public const string PATH = "UserData";

    //bool 반환대신 구조체 만들어서 작업 로그와 성공 여부를 함께 넘기는거 좀더 고려해볼것.
    /// <summary>
    ///  Update UserProfile And Create new UserData on RealtimeDatabase. 
    ///  Return it works done well or not by bool 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool SetName( string name )
    {
        if ( !isValid )
        {
            Debug.Log(VALIDFAIL);
            return isValid;
        }
        bool workFlag = true;

        //UserProfile Update
        UserProfile profile = new UserProfile();
        profile.DisplayName = name;
        Auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if ( task.IsCanceled )
            {
                Debug.Log("UpdateUserProfileAsync Canceled");
                workFlag = false;
                return;
            }
            else if ( task.IsFaulted )
            {
                Debug.LogException(task.Exception);
                workFlag = false;
                return;
            }
            Debug.Log("UpdateUserProfileAsync Success!");
        });

        //Database Set
        UserData userData = new UserData(name);
        string json = JsonUtility.ToJson(userData);
        DB
            .GetReference(PATH)
            .Child(Auth.CurrentUser.UserId)
            .SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if ( task.IsFaulted )
                {
                    Debug.Log($"DB SetValueAsync Faulted : {task.Exception}");
                    workFlag = false;
                    return;
                }
                if ( task.IsCanceled )
                {
                    Debug.Log("DB SetValueAsync Canceled");
                    workFlag = false;
                    return;
                }
            });
        Debug.Log($"SetName Work Finished : {workFlag}");
        return workFlag;
    }

    /// <summary>
    /// Update UserProfile and UserData from RealtimeDatabase 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool UpdateName( string name )
    {
        if ( !isValid )
        {
            Debug.Log(VALIDFAIL);
            return isValid;
        }

        bool workFlag = true;
        //Database Update 
        DB
            .GetReference(PATH)
            .Child(Auth.CurrentUser.UserId)
            .Child("Name")
            .SetValueAsync(name).ContinueWithOnMainThread(task =>
            {
                if ( task.IsFaulted )
                {
                    Debug.LogError($"DB SetValueAsync Faulted : {task.Exception}");
                    return;
                }
                if ( task.IsCanceled )
                {
                    Debug.LogError("DB SetValueAsync Canceled");
                    return;
                }
                Debug.Log("DB Update Success");
            });

        //UserProfile Update
        profile.DisplayName = name;
        Auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if ( task.IsCanceled )
            {
                Debug.LogError("UpdateUserProfileAsync Canceled");
                workFlag = false;
                return;
            }
            else if ( task.IsFaulted )
            {
                Debug.LogError($"UpdateUserProfileAsync failed : {task.Exception.Message}");
                workFlag = false;
                return;
            }
            Debug.Log("UpdateUserProfileAsync Success!");
        });
        return workFlag;
    }

    /// <summary>
    ///  Get Current User's Name from UserProfile
    /// </summary>
    /// <returns></returns>
    public static string GetName()
    {
        if ( !isValid )
            return VALIDFAIL;

        return Auth.CurrentUser.DisplayName;
    }

    public static UserData GetUserData()
    {
        DB
          .GetReference(PATH).Child(Auth.CurrentUser.UserId)
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
                  userData = JsonUtility.FromJson<UserData>(json);
                  return;
              }
          });

        return userData;
    }


    /// <summary>
    /// Update User Recoord, winCount and score
    /// </summary>
    /// <param name="isWin"> true = win game , false or empty  = lose game  </param>
    /// <returns></returns>
    public static bool UpdateRecord( int score = 0, bool isWin = false )
    {
        if ( !isValid )
        {
            Debug.Log(VALIDFAIL);
            return isValid;
        }

        bool workFlag = true;
        //Get Data From Database
        DB
             .GetReference(PATH).Child(Auth.CurrentUser.UserId)
             .GetValueAsync().ContinueWithOnMainThread(task =>
             {
                 if ( task.IsFaulted )
                 {
                     Debug.LogError($"DB GetValueAsync Faulted : {task.Exception}");
                     workFlag = false;
                     return;
                 }
                 if ( task.IsCanceled )
                 {
                     Debug.LogError($"DB SetValueAsync Canceled");
                     workFlag = false;
                     return;
                 }

                 DataSnapshot snapshot = task.Result;
                 if ( snapshot.Exists )
                 {
                     string json = snapshot.GetRawJsonValue();
                     Debug.Log(json);
                     userData = JsonUtility.FromJson<UserData>(json);

                     Debug.Log($"{userData.Name}");
                     return;
                 }
             });

        //Update UserData Value

        if ( isWin )
            userData.winCount++;

        userData.playCount++;
        userData.score += score;

        //Update Database
        string json = JsonUtility.ToJson(userData);

        DB
           .GetReference(PATH)
           .Child(Auth.CurrentUser.UserId)
           .SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
           {
               if ( task.IsFaulted )
               {
                   Debug.LogError($"DB SetValueAsync Faulted : {task.Exception}");
                   workFlag = false;
                   return;
               }
               if ( task.IsCanceled )
               {
                   Debug.LogError("DB SetValueAsync Canceled");
                   workFlag = false;
                   return;
               }
           });

        return workFlag;
    }
}
