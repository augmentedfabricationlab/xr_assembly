using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public GameObject assemblyLoader;

    public DataSaveManager dataSaveManager;
    private DatabaseReference reference;

    public Text selectionText;
    public Slider selectionSlider;
    public Text changeStateButton;
    public GameObject notificationCanvas;
    public Text notificationText;
    public GameObject userNotificationCanvas;
    public Text userNotificationText;
    public Text userInfoText;
    public GameObject tagInfoCanvas;
    public GameObject logoCanvas;
    public GameObject buttonCanvas;
    public GameObject arrow;

    public bool userInfoDisplayed;

    private Vector3 transformVector;
    private Vector3 originDifference;

    public string currentPhase;

    public Dictionary<string, CollectiveAssembly.Node> assemblyNodes;
    public IDictionary<string, GameObject> assemblyMeshes = new Dictionary<string, GameObject>();
    public Dictionary<string, Dictionary<string, CollectiveAssembly.EdgeAttributes>> assemblyAdjacency;
    public Dictionary<string, Dictionary<string, CollectiveAssembly.EdgeAttributes>> assemblyEdge;

    private Quaternion nullQuaternion = new Quaternion(0,0,0,0);

    public void Restart()
    {
        // Start with Phase 1.
        currentPhase = "Phase1";

        // Get the Realtime Database reference.
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        // Start listening to the Realtime Database.
        reference.Child("Built Keys").ChildAdded += dataSaveManager.HandleChildAdded;
        reference.Child("Built Keys").ChildRemoved += dataSaveManager.HandleChildRemoved;
        reference.Child("Users").ChildChanged += dataSaveManager.HandleChildChanged;
        reference.Child("Current Phase").Child("Phase").ValueChanged += dataSaveManager.HandlePhaseChanged;

        // Get the assembly data.
        assemblyNodes = assemblyLoader.GetComponent<CollectiveAssemblyLoader>().assemblyNodes;
        assemblyMeshes = assemblyLoader.GetComponent<CollectiveAssemblyLoader>().assemblyMeshes;
        assemblyAdjacency = assemblyLoader.GetComponent<CollectiveAssemblyLoader>().assemblyAdjacency;
        assemblyEdge = assemblyLoader.GetComponent<CollectiveAssemblyLoader>().assemblyEdge;

        // Check and build according to the built keys before.
        CheckForBuiltStates();

        // Invoke events.
        dataSaveManager.OnBuiltUpdate.AddListener(UpdateBuiltStates);
        dataSaveManager.OnRemovedUpdate.AddListener(UpdateRemovedStates);
        dataSaveManager.OnSelectedKeyUpdate.AddListener(UpdateOtherSelectedElements);
        dataSaveManager.OnPhaseUpdate.AddListener(UpdateCurrentPhase);

        // Load other users and generate the user data of own.
        dataSaveManager.LoadUsers();

        // Visualise the colors of the elements.
        VisualiseStates();

        tagInfoCanvas.SetActive(true);

        // Set the selection slider.
        selectionSlider.maxValue = GetKeysBuildable().Count - 1;
        Debug.Log("SELECTION SLIDER MAX: " + selectionSlider.maxValue);

        userInfoDisplayed = false;
    }

    // Stop listeening to the Realtime Database on app closed. - UNUSED
    //private void OnDestroy()
    //{
    //    //reference.Child("ASSEMBLY_STATE").ValueChanged -= dataSaveManager.HandleValueChanged;
    //    reference.Child("Built Keys").ChildAdded -= dataSaveManager.HandleChildAdded;
    //    reference.Child("Built Keys").ChildRemoved -= dataSaveManager.HandleChildRemoved;
    //    reference = null;
    //}

    // Check if element is already built.
    public void CheckForBuiltStates()
    {
        foreach (var key in assemblyNodes.Keys)
        {
            if (assemblyNodes[key].IsBuilt == true)
            {
                dataSaveManager.WriteNewElement(key);

                Debug.Log(key + " element was already pre-built.");
            }
        }
    }

    // Get the elements according to the current phase.
    public List<string> GetCurrentPhaseKeys()
    {
        List<string> CurrentPhaseKeys = new List<string>();

        foreach (var key in assemblyNodes.Keys)
        {
            if (assemblyNodes[key].Phase3 == true && currentPhase == "Phase3")
            {
                CurrentPhaseKeys.Add(key);
            }
            else if (assemblyNodes[key].Phase2 == true && currentPhase == "Phase2")
            {
                CurrentPhaseKeys.Add(key);
            }
            else
            {
                CurrentPhaseKeys.Add(key);
            }
        }
        return CurrentPhaseKeys;
    }

    // JUST FOR TESTING
    // Preset states
    public void PresetPhase()
    {
        List<string> KeysNeigborsBelow = new List<string>();

        foreach (var key in assemblyNodes.Keys.Where(key => (assemblyNodes[key].Phase2 == true) && (assemblyNodes[key].Phase1 == true)))
        {
            assemblyNodes[key].IsBuilt = true;
        }
    }

    // Update is_built = true as soon as the could data changes.
    public void UpdateBuiltStates()
    {
        if (assemblyLoader.GetComponent<CollectiveAssemblyLoader>().assemblyLoaded)
        {
            if (assemblyNodes[dataSaveManager.newBuiltKey].IsBuilt != true)
            {
                assemblyNodes[dataSaveManager.newBuiltKey].IsBuilt = true;
                StartCoroutine(DisplayTimedNotification($"Element {dataSaveManager.newBuiltKey} is built."));
                Debug.Log(dataSaveManager.newBuiltKey + " is built.");
            }
        }

        VisualiseStates();

        selectionSlider.maxValue = GetKeysBuildable().Count - 1;
        Debug.Log("UPDATED SELECTION SLIDER MAX: " + selectionSlider.maxValue);
    }

    // Update is_built = false as soon as the could data changes.
    public void UpdateRemovedStates()
    {
        if (assemblyLoader.GetComponent<CollectiveAssemblyLoader>().assemblyLoaded)
        {
            if (assemblyNodes[dataSaveManager.newRemovedKey].IsBuilt == true)
            {
                assemblyNodes[dataSaveManager.newRemovedKey].IsBuilt = false;
                StartCoroutine(DisplayTimedNotification($"Element {dataSaveManager.newRemovedKey} is removed."));
                Debug.Log(dataSaveManager.newBuiltKey + " is removed.");
            }
        }

        VisualiseStates();

        selectionSlider.maxValue = GetKeysBuildable().Count - 1;
        Debug.Log("UPDATED SELECTION SLIDER MAX: " + selectionSlider.maxValue);
    }

    // Update from the current phase in the Realtime Database when changed.
    public void UpdateCurrentPhase()
    {
        currentPhase = "Phase" + dataSaveManager.newCurrentPhase;
        Debug.Log("Current phase is " + currentPhase);

        VisualiseStates();

        selectionSlider.maxValue = GetKeysBuildable().Count - 1;
        Debug.Log("UPDATED SELECTION SLIDER MAX: " + selectionSlider.maxValue);
    }

    // Return the keys of the elements which are buildable.
    // i.e., the elements which are supported by the ground or by at least one elements below
    public List<string> GetKeysBuildable()
    {
        List<string> keysBuildable = new List<string>();
        List<string> CurrentPhaseKeys = GetCurrentPhaseKeys();

        foreach (var key in assemblyNodes.Keys.Where(key => (assemblyNodes[key].IsBuilt != true) && CurrentPhaseKeys.Contains(key)))
        {
            if (assemblyNodes[key].IdxV == 0) // Check if the element is supported by the ground.
            {
                keysBuildable.Add(key);
            }
            else
            {
                List<string> neighborsBelow = GetKeysNeighborsBelow(key);

                if (neighborsBelow.Count > 0)
                {
                    List<bool> isBuilt = new List<bool>();

                    foreach (var neighborKey in neighborsBelow)
                    {
                        if (assemblyNodes[neighborKey].IsBuilt != true)
                        {
                            isBuilt.Add(false);
                        }
                        else
                        {
                            isBuilt.Add(true);
                        }
                    }

                    bool supported = !isBuilt.Contains(false);

                    if (supported == true)
                    {
                        keysBuildable.Add(key);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        // Order the keysBuildable list.
        keysBuildable = keysBuildable.OrderBy(x => x.Length).ThenBy(x => x).ToList();
        return keysBuildable;
    }

    // Return the keys of the elements which are removable,
    // i.e., the elements which are used for support and are already built
    public List<string> GetKeysRemovable()
    {
        List<string> keysRemovable = new List<string>();

        foreach (var key in assemblyNodes.Keys.Where(key => (assemblyNodes[key].IsBuilt == true) && (assemblyNodes[key].IsSupport == true)))
        {
            if (assemblyNodes[key].IdxV == 9) // Check if the element is in the top support layer.
            {
                keysRemovable.Add(key);
            }
            else
            {
                List<string> neighborsBelow = GetKeysNeighborsAbove(key);

                if (neighborsBelow.Count > 0)
                {
                    List<bool> isBuilt = new List<bool>();
                    foreach (var neighborKey in neighborsBelow)
                    {
                        if (assemblyNodes[neighborKey].IsBuilt != true)
                        {
                            isBuilt.Add(false);
                        }
                        else
                        {
                            isBuilt.Add(true);
                        }
                    }
                    bool removable = !isBuilt.Contains(true); // Check if all neighbors above are not built.

                    if (removable == true)
                    {
                        keysRemovable.Add(key);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

        }

        // Order the keysRemovable list.
        keysRemovable = keysRemovable.OrderBy(x => x.Length).ThenBy(x => x).ToList();
        return keysRemovable;
    }

    // Get the keys of all elements that has to be dismantaled. - UNUSED (not working)
    //public List<string> GetKeysDismantable()
    //{
    //    List<string> KeysDismantable = new List<string>();

    //    foreach (var key in assemblyNodes.Keys.Where(key => assemblyNodes[key].IsSupport == true))
    //    {
    //        List<string> NeighborsAbove = GetKeysNeighborsAbove(key);

    //        if (NeighborsAbove.Count > 0)
    //        {
    //            List<bool> isSupport = new List<bool>();

    //            foreach (var nkey in NeighborsAbove)
    //            {
    //                if (assemblyNodes[key].IsSupport != true)
    //                {
    //                    isSupport.Add(false);
    //                }
    //                else
    //                {
    //                    isSupport.Add(true);
    //                }
    //                bool isEdge = isSupport.Contains(false);

    //                if (isEdge == true)
    //                {
    //                    KeysDismantable.Add(key);
    //                }
    //                else
    //                {
    //                    continue;
    //                }
    //            }
    //        }
    //    }
    //    return KeysDismantable;
    //}

    // Get the keys of all elements that has to be mounted to the ground.
    public List<string> GetKeysMounted()
    {
        List<string> KeysMounted = new List<string>();

        foreach (var key in assemblyNodes.Keys.Where(key => (assemblyNodes[key].IsSupport != true) && (assemblyNodes[key].IdxV == 0)))
        {
            KeysMounted.Add(key);
        }

        return KeysMounted;
    }

    // Visualise the colors of the elements according to their states.
    public void VisualiseStates()
    {
        List<string> keysBuildable = GetKeysBuildable();
        List<string> keysRemovable = GetKeysRemovable();
        List<string> keysDismantable = new List<string>();
        string[] dismantableKeys = { "6", "16", "20", "29", "38", "47", "52", "60", "70", "78", "84", "91", "102", "109", "116", "122", "134", "140", "148", "153", "166", "171", "180", "184", "198", "202", "212", "215", "230", "233", "244", "246", "262", "264", "276", "277", "294", "295", "308" }; // Temporary until getKeysDismantable().
        keysDismantable.AddRange(dismantableKeys);

        foreach (var key in assemblyNodes.Keys)
        {
            Material mat;

            // Built elements are blue.
            if (assemblyNodes[key].IsBuilt == true)
            {
                if (currentPhase == "Phase3" && keysRemovable.Contains(key)) // assemblyNodes[key].IsSupport == true
                {
                    mat = Resources.Load("dark blue") as Material;
                }
                else
                {
                    mat = Resources.Load("blue") as Material;
                }
            }
            // Dismantable elements are purple.
            //else if (keysDismantable.Contains(key))
            //{
            //    mat = Resources.Load("purple") as Material;
            //}
            // Buildable elements are light green.
            else if (keysBuildable.Contains(key))
            {
                mat = Resources.Load("green") as Material;
            }
            // Rest of the elements are grey.
            else
            {
                mat = Resources.Load("grey") as Material;
            }

            assemblyMeshes[key].GetComponent<MeshRenderer>().material = mat;

            assemblyMeshes[key].TryGetComponent(out Outline exline);

            exline.enabled = false;
        }

        VisualiseOtherSelectedElements();
    }

    // Visualise the elements that are selected by other users.
    public void VisualiseOtherSelectedElements()
    {
        Dictionary<string, string> selectedElements = new Dictionary<string, string>();

        selectedElements = dataSaveManager.GetComponent<DataSaveManager>().selectedKeys;

        // Others' selected elements are dark green.
        Material mat = Resources.Load("dark green") as Material;

        if (selectedElements != null)
        {
            foreach (KeyValuePair<string, string> selected in selectedElements)
            {
                Debug.Log(selected.Key + selected.Value);

                assemblyMeshes[selected.Value].GetComponent<MeshRenderer>().material = mat;
            }
        }
    }

    // Update when others change their selected elements.
    public void UpdateOtherSelectedElements()
    {
        var value = selectionSlider.value;
        VisualiseStates();
        SelectElement((int)value);
    }

    // Choose which states to visualise using the dropdown.
    public void ShowStates(int dropdownValue)
    {
        List<string> keysBuildable = GetKeysBuildable();

        // Dropdown values:
        // 0 : "Show Only Built"
        // 1 : "Show Built + Buildable"
        // 2 : "Show Built + Current Phase"
        // 3 : "Show All"

        foreach (var key in assemblyNodes.Keys)
        {
            assemblyMeshes[key].SetActive(false);

            if (assemblyNodes[key].IsBuilt == true)
            {
                assemblyMeshes[key].SetActive(true);
            }
            else if (keysBuildable.Contains(key) && dropdownValue == 1)
            {
                assemblyMeshes[key].SetActive(true);
            }
            else if (dropdownValue == 2)
            {
                if (currentPhase == "Phase1" && assemblyNodes[key].Phase1 == true)
                {
                    assemblyMeshes[key].SetActive(true);
                } else if (currentPhase == "Phase2" && assemblyNodes[key].Phase2 == true)
                {
                    assemblyMeshes[key].SetActive(true);
                } else if (currentPhase == "Phase3" && assemblyNodes[key].Phase3 == true)
                {
                    assemblyMeshes[key].SetActive(true);
                }
            }
            else if (dropdownValue == 3 && assemblyNodes[key].IsSupport != true)
            {
                assemblyMeshes[key].SetActive(true);
            }
            else
            {
                assemblyMeshes[key].SetActive(false);
            }
        }
    }

    // Select one element from all buildable and removable (if currentPhase == "Phase3") elements.
    // Set changeStateButton.text to "BUILD" or "REMOVE" accordingly.
    public void SelectElement(int value)
    {
        changeStateButton.text = "Build";

        Material mat = Resources.Load("red") as Material;

        List<string> keysBuildable = GetKeysBuildable();
        List<string> keysRemovable = GetKeysRemovable();
        List<string> keysDismantable = new List<string>();
        string[] dismantableKeys = { "6", "16", "20", "29", "38", "47", "52", "60", "70", "78", "84", "91", "102", "109", "116", "122", "134", "140", "148", "153", "166", "171", "180", "184", "198", "202", "212", "215", "230", "233", "244", "246", "262", "264", "276", "277", "294", "295", "308" };
        keysDismantable.AddRange(dismantableKeys);
        List<string> keysMounted = GetKeysMounted();

        string key;

        try
        {
            if (currentPhase == "Phase3")
            {
                List<string> keysMerged = keysBuildable.Concat(keysRemovable).ToList();
                keysMerged = keysMerged.OrderBy(x => x.Length).ThenBy(x => x).ToList();
                key = keysMerged[value];

                if (keysRemovable.Contains(key))
                {
                    changeStateButton.text = "Remove";
                }
            }
            else
            {
                key = keysBuildable[value];
            }

            assemblyMeshes[key].GetComponent<MeshRenderer>().material = mat;

            var outline = assemblyMeshes[key].GetComponent<Outline>();

            outline.enabled = true;

            dataSaveManager.WriteSelectedElement(key);

            selectionText.text = key;

            if (keysDismantable.Contains(key))
            {
                DisplayNotification($"Place dismantable for element {key}!");
            } else if (keysMounted.Contains(key))
            {
                DisplayNotification($"Mount element {key} to the ground!");
            } else
            {
                notificationCanvas.SetActive(false);
            }

        } catch
        {
            selectionText.text = "0";
            selectionSlider.value = 0;
            Debug.Log("Key is out of range.");
        }

        // Set the arrow following selected element active. - UNUSED
        //    arrow.SetActive(true);

        //    //correct one:
        //    Vector3 originFrame = new Vector3(
        //        (float)-assemblyNodes[key].Element.Frame.Point[1],
        //        (float)assemblyNodes[key].Element.Frame.Point[2],
        //        (float)assemblyNodes[key].Element.Frame.Point[0]);
        //    arrow.transform.position = originFrame + assemblyMeshes["0"].transform.position;
        //    //arrow.transform.RotateAround(assemblyMeshes[key].transform.position, Vector3.up, 90);

    }

    // Change the built or removed state of the element once an element is built or removed by the user.
    public void ChangeState(int value)
    {
        List<string> keysBuildable = GetKeysBuildable();

        List<string> keysRemovable = GetKeysRemovable();

        string key;

        if (currentPhase == "Phase3")
        {
            List<string> keysMerged = keysBuildable.Concat(keysRemovable).ToList();
            keysMerged = keysMerged.OrderBy(x => x.Length).ThenBy(x => x).ToList();
            key = keysMerged[value]; 
        } else
        {
            key = keysBuildable[value];
        }

        if (assemblyNodes[key].IsBuilt != true && keysBuildable.Contains(key))
        {
            assemblyNodes[key].IsBuilt = true;

            dataSaveManager.WriteNewElement(key);

            StartCoroutine(DisplayTimedNotification($"You have built element {key}."));

            Debug.Log(key + " element is built by me.");
        }

        else if (assemblyNodes[key].IsBuilt == true && keysRemovable.Contains(key))
        {
            assemblyNodes[key].IsBuilt = false;

            dataSaveManager.RemoveNewElement(key);

            StartCoroutine(DisplayTimedNotification($"You have removed element {key}."));

            Debug.Log(key + " element is removed by me.");
        }

        selectionSlider.maxValue = GetKeysBuildable().Count - 1;
        Debug.Log("UPDATED SELECTION SLIDER MAX: " + selectionSlider.maxValue);
    }

    // Display the notification line on top of the screen. - timed
    public IEnumerator DisplayTimedNotification(string message, Color? textColor = null)
    {
        if (textColor == null) textColor = Color.black;

        notificationCanvas.SetActive(false);

        if (!notificationCanvas.activeSelf)
        {
            notificationText.color = (Color)textColor;
            notificationText.text = message;
            notificationCanvas.SetActive(true);
            yield return new WaitForSeconds(3);
            notificationCanvas.SetActive(false);
        }
    }

    // Display the notification line on bottom of the screen.
    public void DisplayNotification(string message, Color? textColor = null)
    {
        if (textColor == null) textColor = new Color(0.8113208f, 0.3712175f, 0.4175107f, 1f);

        notificationCanvas.SetActive(false);

        if (!notificationCanvas.activeSelf)
        {
            notificationText.color = (Color)textColor;
            notificationText.text = message;
            notificationCanvas.SetActive(true);
        }
    }

    // Display the user info in the beginning in the middle of the screen.
    public void StartDisplay()
    {
        StartCoroutine(UserNotification());
        userInfoDisplayed = true;
    }

    public IEnumerator UserNotification()
    {
        string ID = dataSaveManager.myUserID;

        if (!userNotificationCanvas.activeSelf)
        {
            userNotificationText.text = "User " + ID + "!";
            userNotificationCanvas.SetActive(true);
            yield return new WaitForSeconds(5);
            userNotificationCanvas.SetActive(false);
        }
        userInfoText.text = "User " + ID;
    }

    // Get the direct neighbors of a node above.
    public List<string> GetKeysNeighborsAbove(string key)
    {
        List<string> KeysNeigborsAbove = new List<string>();
        List<string> KeysNeghborsIn = assemblyAdjacency[key].Keys.Except(assemblyEdge[key].Keys).ToList();

        for (int i = 0; i < KeysNeghborsIn.Count; i++)
        {
            KeysNeigborsAbove.Add(KeysNeghborsIn[i]);
        }
        return KeysNeigborsAbove;
    }

    // Get the direct neighbors of a node below.
    public List<string> GetKeysNeighborsBelow(string key)
    {
        List<string> KeysNeigborsBelow = new List<string>();
        List<string> KeysNeightborsOut = new List<string>(assemblyEdge[key].Keys);

        for (int i = 0; i < KeysNeightborsOut.Count; i++)
        {
            KeysNeigborsBelow.Add(KeysNeightborsOut[i]);
        }
        return KeysNeigborsBelow;
    }
}

