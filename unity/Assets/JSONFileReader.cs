using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONFileReader
{
   public static string LoadJsonAsResource(string path)
    {
        string jsonFilePath = path.Replace(".json", "");
        TextAsset loadedJsonFile = Resources.Load<TextAsset>(jsonFilePath);
        return loadedJsonFile.text;
    }
}
