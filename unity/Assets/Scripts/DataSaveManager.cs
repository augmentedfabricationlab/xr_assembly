using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Handle the Firebase Realtime Database for realtime actions.
public class DataSaveManager : MonoBehaviour
{
    public ProgressManager progressManager;
    private DatabaseReference reference;

    private const string BUILT_KEYS = "Built Keys";
    private const string USERS = "Users";

    public string newBuiltKey;
    public string newRemovedKey;
    public string newCurrentPhase;

    public UserData myUserData;
    public string myUserID;
    public List<int> othersUserID = new List<int>();
    public Dictionary<string, string> selectedKeys = new Dictionary<string, string>();

    public UnityEvent OnBuiltUpdate = new UnityEvent();
    public UnityEvent OnRemovedUpdate = new UnityEvent();
    public UnityEvent OnSelectedKeyUpdate = new UnityEvent();
    public UnityEvent OnPhaseUpdate = new UnityEvent();

    private void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Destroy the user data when app is closed.
    private void OnDestroy()
    {
        EraseUserData("User_" + myUserData.userID.ToString());
    }


        // Save the user data of own.
    public void SaveUserData()
    {
        string USER_KEY = "User_" + myUserData.userID.ToString();
        reference.Child(USERS).Child(USER_KEY).SetRawJsonValueAsync(JsonUtility.ToJson(myUserData));
    }

    // Erase the user data of own.
    public void EraseUserData(string USER)
    {
        try
        {
            reference.Child(USERS).Child(USER).RemoveValueAsync();
        } catch
        {
            Debug.LogWarning("User cannot be deleted.");
        }
        
    }

    // Load the data of all other users and generate the user data of own.
    public void LoadUsers()
    {
        othersUserID.Clear();
        Debug.Log("Loading users.");

        reference.Child(USERS).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Task is faulted.");
                return;
            }
            else if (task.IsCompleted)
            {
                try
                {
                    var dataSnapshot = task.Result;

                    foreach (DataSnapshot snapShot in dataSnapshot.Children)
                    {
                        var userjson = snapShot.GetRawJsonValue();
                        UserData user = JsonUtility.FromJson<UserData>(userjson);
                        int ID = user.GetUserID();
                        othersUserID.Add(ID);
                    }

                    if (othersUserID.Count == 0)
                    {
                        myUserData.userID = 1;
                        Debug.Log($"User {myUserData.userID} is generated.");
                        SaveUserData();
                    }
                    else
                    {
                        int myID = othersUserID.Max() + 1;
                        myUserData.userID = myID;
                        myUserID = myID.ToString();
                        Debug.Log($"User {myUserData.userID} is generated.");
                        SaveUserData();
                    }
                }
                catch
                {
                    Debug.LogWarning("Users could not be loaded.");
                }

            }
        });
    }

    // Write the selected element key to the user data.
    public void WriteSelectedElement(string element)
    {
        string USER_KEY = "User_" + myUserData.userID.ToString();
        reference.Child(USERS).Child(USER_KEY).Child("selectedKey").SetValueAsync(element);
    }

    // When the selected key of others change, invoke "on selected key update".
    public void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        var dataSnapshot = args.Snapshot;
        try
        {
            if (!dataSnapshot.Key.Contains(myUserData.userID.ToString()))
            {
                var userjson = dataSnapshot.GetRawJsonValue();
                UserData user = JsonUtility.FromJson<UserData>(userjson);
                int userID = user.GetUserID();
                string selectedKey = user.GetSelectedKey();

                if (selectedKeys.ContainsKey(userID.ToString()))
                {
                    selectedKeys[userID.ToString()] = selectedKey;
                }
                else
                {
                    selectedKeys.Add(userID.ToString(), selectedKey);
                }
                OnSelectedKeyUpdate.Invoke();
            }
        } catch
        {
            Debug.LogWarning("The selected element could not be retrieved.");
        }

    }

    // Check if the save for the built keys data exists.
    public async Task<bool> SaveExists()
    {
        var dataSnapshot = await reference.Child(BUILT_KEYS).GetValueAsync();
        return dataSnapshot.Exists;
    }

    // Write the new built element key to the dictionary of the built keys.
    public void WriteNewElement(string element)
    {
        //string key = reference.Child(BUILT_KEYS).Push().Key;

        reference.Child(BUILT_KEYS).Child(element).SetValueAsync(element);
    }

    // Remove the new removed element key from the dictionary of the built keys.
    public void RemoveNewElement(string element)
    {
        try
        {
            reference.Child(BUILT_KEYS).Child(element).RemoveValueAsync();
        } catch
        {
            Debug.LogWarning("Element could not be removed.");
        }
        
    }

    // When a new built key is added to the dictionary of the built keys, invoke "on built update".
    public void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        var value = args.Snapshot.Value;

        try
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                newBuiltKey = value.ToString();
                OnBuiltUpdate.Invoke();
                Debug.Log(value + " element is built by someone else.");
            }
        } catch
        {
            Debug.LogWarning("Built element could not be retrieved.");
        }

    }

    // When a built key is removed from the dictionary of the built keys, invoke "on removed update".
    public void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        var value = args.Snapshot.Value;

        try
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                newRemovedKey = value.ToString();
                OnRemovedUpdate.Invoke();
                Debug.Log(value + " element is removed by someone else.");
            }
        } catch
        {
            Debug.LogWarning("Removed element could not be retrieved.");
        }

    }
}
