using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Vuforia;

public class Controls : MonoBehaviour
{
    public GameObject workplace; // Workplace as a child of image targets, replacing as the digital model.

    Button thisButton;
    public Button playButton;
    bool playButtonIsOn = false;

    public GameObject downloadCanvas;
    public GameObject newProjectCanvas;
    public GameObject infoCanvas;
    public GameObject logoCanvas;
    public GameObject buttonCanvas;
    public GameObject tagInfoCanvas;

    public GameObject projectInfoCanvas;
    public GameObject Phase1Panel;
    public GameObject Phase2Panel;
    public GameObject Phase3Panel;

    public GameObject assemblyLoader;
    public GameObject progressManager;

    public Slider selectionSlider;

    public Dropdown visualisationDropdown;

    // Start is called before the first frame update.
    void Start()
    {
        // Set the workplace/digital model active.
        workplace.SetActive(false);

        assemblyLoader.GetComponent<XRAssemblyLoader>().assemblyLoaded = false;
        assemblyLoader.GetComponent<XRAssemblyLoader>().scalingFactor = 1;
        thisButton = GetComponent<Button>();
        TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
    }

    // Display the download canvas to load the json files.
    public void DisplayDownloadCanvas()
    {
        if (!assemblyLoader.GetComponent<XRAssemblyLoader>().assemblyLoaded)
        {
            if (!downloadCanvas.activeSelf)
            {
                infoCanvas.SetActive(false);
                downloadCanvas.SetActive(true);
            }
            else
            {
                downloadCanvas.SetActive(false);
            }
        }
        else
        {
            if (!newProjectCanvas.activeSelf)
            {
                newProjectCanvas.SetActive(true);
            } else
            {
                newProjectCanvas.SetActive(false);
            }
        }
    }

    // Display the info canvas for information about the app and credits.
    public void DisplayInfoCanvas()
    {
        if (!infoCanvas.activeSelf)
        {
            downloadCanvas.SetActive(false);
            infoCanvas.SetActive(true);
        }
        else
        {
            infoCanvas.SetActive(false);
            downloadCanvas.SetActive(true);
        }
    }

    // Display the project info canvas for information about the project and its progress.
    public void DisplayProjectInfoCanvas()
    {
        HighlightCurrentPhaseInfo();

        if (!projectInfoCanvas.activeSelf)
        {
            projectInfoCanvas.SetActive(true);
        }
        else
        {
            projectInfoCanvas.SetActive(false);
        }
    }

    // Control the build/remove button.
    public void ChangeStateButton(Slider slider)
    {
        progressManager.GetComponent<ProgressManager>().ChangeState((int)slider.value);
        VisualisationDropdown();
    }

    // Control the slider to select elements.
    public void NewSelectionSliderInput(Slider slider)
    {
        progressManager.GetComponent<ProgressManager>().VisualiseStates();
        progressManager.GetComponent<ProgressManager>().SelectElement((int)slider.value);
    }

    // Control the dropdown to change the visualised states.
    public void VisualisationDropdown()
    {
        int index = visualisationDropdown.value;
        progressManager.GetComponent<ProgressManager>().ShowStates((int)index);
    }

    // Show the workplace/digital model.
    public void ShowWorkplace()
    {
        if (!workplace.activeSelf)
        {
            workplace.SetActive(true);
        }

        if (!buttonCanvas.activeSelf)
        {
            buttonCanvas.SetActive(true);
        }

        logoCanvas.SetActive(false);

        tagInfoCanvas.SetActive(false);

        if (progressManager.GetComponent<ProgressManager>().userInfoDisplayed == false)
        {
            progressManager.GetComponent<ProgressManager>().StartDisplay();
        }
        
    }

    // Load new project.
    public void LoadNewProject()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // Don't load new project.
    public void DontLoadNewProject()
    {
        newProjectCanvas.SetActive(false);
    }

    // Highlight the current phase in the project info canvas.
    public void HighlightCurrentPhaseInfo()
    {
        string currentPhase = progressManager.GetComponent<ProgressManager>().currentPhase;
        string COMPLETED = "COMPLETED!";
        string INPROGRESS = "IN PROGRESS...";
        string INCOMING = "INCOMING...";
        Text Phase1Text = Phase1Panel.transform.Find("Progress").GetComponent<Text>();
        Text Phase2Text = Phase2Panel.transform.Find("Progress").GetComponent<Text>();
        Text Phase3Text = Phase3Panel.transform.Find("Progress").GetComponent<Text>();
        Color lightRed = new UnityEngine.Color(255f, 0f, 0f, 0.2f);
        Color offWhite = new UnityEngine.Color(1f, 1f, 1f, 0.2f);

        Phase1Panel.GetComponent<UnityEngine.UI.Image>().color = offWhite;
        Phase2Panel.GetComponent<UnityEngine.UI.Image>().color = offWhite;
        Phase3Panel.GetComponent<UnityEngine.UI.Image>().color = offWhite;

        if (currentPhase == "Phase1")
        {
            Phase1Panel.GetComponent<UnityEngine.UI.Image>().color = lightRed;
            Phase1Text.text = INPROGRESS;
            Phase2Text.text = INCOMING;
            Phase3Text.text = INCOMING;
        }
        else if (currentPhase == "Phase2")
        {
            Phase2Panel.GetComponent<UnityEngine.UI.Image>().color = lightRed;
            Phase1Text.text = COMPLETED;
            Phase2Text.text = INPROGRESS;
            Phase3Text.text = INCOMING;
        }
        else
        {
            Phase3Panel.GetComponent<UnityEngine.UI.Image>().color = lightRed;
            Phase1Text.text = COMPLETED;
            Phase2Text.text = COMPLETED;
            Phase3Text.text = INPROGRESS;
        }
    }
}
