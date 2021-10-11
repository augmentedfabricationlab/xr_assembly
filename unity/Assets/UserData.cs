using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// User data class as it is stored in the Firebase Realtime Database.
[Serializable]
public class UserData {

    public int userID;
    public string selectedKey;

    public int GetUserID()
    {
        return userID;
    }

    public string GetSelectedKey()
    {
        return selectedKey;
    }
}
