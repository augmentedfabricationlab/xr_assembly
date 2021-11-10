using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static ProgressManager;
using Vuforia;
using UnityEngine.SceneManagement;
using Firebase.Storage;
using Firebase.Extensions;
using System;

// Load the json file from the Firabase Storage or GitHub or URL.
public class LoadFile : MonoBehaviour
{
    public GameObject assemblyGenerator;

    public string default_url = "https://raw.githubusercontent.com/augmentedfabricationlab/assembly_json_collection/master/files/";
    public string downloadFromUrl;
    private string defaultFirebasePath = "gs://ar-fabrication.appspot.com"; // Project specific Firebase Storage path link.

    public InputField FileInputField;
    public string fileText = "";

    public Button FromURL;
    public Button FromGitHub;
    public Button FromFirebase;

    public GameObject errorMessage;
    public GameObject progressManager;
    public GameObject imageTarget;
    private float progress;
    private bool loadFromURL;
    private bool loadFromFirebase;

    private void Start()
    {
        downloadFromUrl = default_url;
        FromFirebase.Select();
    }

    // Select "From Firebase" button.
    public void LoadFromFirebase()
    {
        FileInputField.gameObject.SetActive(true);
        loadFromURL = false;
        loadFromFirebase = true;
    }

    // Select "From GitHub" button.
    public void LoadFromGitHub()
    {
        FileInputField.gameObject.SetActive(true);
        loadFromURL = false;
        loadFromFirebase = false;
    }

    // Select "From URL" button. - UNUSED
    public void LoadFromURL()
    {
        FileInputField.gameObject.SetActive(false);
        loadFromURL = true;
        loadFromFirebase = false;
    }

    // Load the json file according to the source selected.
    public void Load()
    {
        StorageReference fileRef = GetFirebase(FileInputField.text);

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
        }
        else
        {
            if (loadFromFirebase)
            {
                fileRef.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
                {
                    if (!task.IsFaulted && !task.IsCanceled)
                    {
                        Debug.Log("Cloud URL: " + task.Result);
                        StartCoroutine(GetTextFromUrl(task.Result.ToString()));
                    }
                });
            }
            else
            {
                string finalUrl = downloadFromUrl + FileInputField.text;
                Debug.Log("New main url is: " + finalUrl);
                StartCoroutine(GetTextFromUrl(finalUrl));
            }
        }
    }

    // Define the Firebase Storage reference with the file name.
    private StorageReference GetFirebase(string fileName)
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageRef = storage.GetReferenceFromUrl(defaultFirebasePath);
        StorageReference fileRef = storageRef.Child(fileName);

        return fileRef;
    }

    // Get the json text from the constructed URL.
    private IEnumerator GetTextFromUrl(string _url)
    {
        UnityWebRequest www = UnityWebRequest.Get(_url);
        var asyncOperation = www.SendWebRequest();

        while (!www.isDone)
        {
            progress = asyncOperation.progress;
            yield return null;
        }

        progress = 1f;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogWarning("Error. URL is empty!");
            errorMessage.SetActive(true);
        }
        else
        {
            fileText = www.downloadHandler.text; // Get the json text.

            try
            {
            assemblyGenerator.GetComponent<XRAssemblyLoader>().generateAssembly(fileText); // Generate the assembly model using the json text.
            errorMessage.SetActive(false);
            transform.gameObject.SetActive(false);
            Debug.Log("Assembly is generated.");
            }
            catch
            {
                errorMessage.SetActive(true);
                Debug.LogWarning("Error. Could not generate assembly.");
            }

        }       
    }

    // Change the default URL in case source is URL. - UNUSED
    public void ChangeDefaultURL(InputField inputField)
    {
        if (inputField.text.Equals("") && !loadFromURL)
        {
            downloadFromUrl = default_url;
        }
        else if (!inputField.text.Equals("") && !loadFromURL)
        {
            downloadFromUrl = "https://raw.githubusercontent.com/" + inputField.text + "/";
        }
        else
        {
            downloadFromUrl = inputField.text;
        }
    }
}
