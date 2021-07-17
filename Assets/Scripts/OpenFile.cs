using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class OpenFile : MonoBehaviour
{
    public string Path;

    [SerializeField]
    TMP_Text _pathDisplay;

    [SerializeField]
    TMP_Text _statusDisplay;

    [SerializeField]
    GameObject _controller;

    [SerializeField]
    TMP_Dropdown _accIDDropdown;

    [SerializeField]
    TextAsset _profileAccID;

    [SerializeField]
    private string _defaultOutputName;

    private int optionsCount = 0;



    public Dictionary<int, List<string>> _profiles = new Dictionary<int, List<string>>();
    public void Click()
    {
        //Open file to find path
        FileBrowser.SetDefaultFilter(".csv");
       StartCoroutine(ShowLoadDialogCoroutine());

    }



    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: both, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");
        if (FileBrowser.Success)
        {
            // Writes paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            Path = FileBrowser.Result[0];
            OpenFilePath();
        }
    }

    void OpenFilePath()
    {
        _pathDisplay.text = Path;
        Debug.Log("Selected: " + Path);
        //Convert to string and pass to next script
        if (File.Exists(Path))
        {
            byte[] m_bytes = File.ReadAllBytes(Path);

            string s = System.Text.Encoding.UTF8.GetString(m_bytes);

            _statusDisplay.text = "Read Success";

            RegexFormat _nextScript = _controller.GetComponent<RegexFormat>();
             _nextScript.FormatCSVtoQFX(s);
        }
        else
        {
            _statusDisplay.text = "Path not Found";
        }
    }


    void PopulateDropDown()
    {
        //Import profile.txt
        byte[] m_bytes = File.ReadAllBytes(Application.streamingAssetsPath + "\\Profile.txt");
        print(Application.streamingAssetsPath + "Profile.txt");
        string profileAsset = System.Text.Encoding.UTF8.GetString(m_bytes);
       
       // string profileAsset = _profileAccID.ToString();
        string[] separatingStrings = { "||", "DEFERRED SALES CHARGE" };
        string[] profileLines = profileAsset.Split(separatingStrings, 0);

        int lineCount = 0;
        int wordCount = 0;

        //Format string into variables
        foreach (string lineOrig in profileLines)
        {

            string line = lineOrig.Replace("\r", "").Replace("\n", "");
            string[] sep = { "//" };
            string[] wordarr = line.Split(',');


            wordCount = 0;
            string name = "";
            string accID = "";
            //Assign variables as word basaed on word count
            foreach (string wordOrig in wordarr)
            {
                
                string word = wordOrig.Replace("\r", "").Replace("\n", "");
                switch(wordCount)
                {
                    case 0:
                        name = word;
                        break;
                    case 1:
                        accID = word;
                        break;
                }
                wordCount += 1;
            }
            lineCount += 1;
            AddDataEntryAndDropdown(new object[] { lineCount, name, accID });
        }


        //Create a list of strings from dictionary
        List<string> options = new List<string>();
        options.Insert(0, "No Profile");
        int i = 1;
        
        foreach (KeyValuePair<int, List<string>> profile in _profiles)
        {
            options.Insert(i, profile.Value[0]);
            optionsCount += 1;
        }
            
        //Populate dropdown from list
        _accIDDropdown.ClearOptions();
        _accIDDropdown.AddOptions(options);
    }

    private void Start()
    {
        
    //Populate dropdown from profile txt file
    PopulateDropDown();
    }
    private void Awake()
    {
        StaticData.OutputName = _defaultOutputName;
        StaticData.isRemovingWhole = false;
        StaticData.isUsingFileName = false;
        StaticData.isUsingProfiles = false;
        StaticData.isWritingFileNameAsACCID = false;
    }
    void AddDataEntryAndDropdown(params object[] entry)
    {
        //Initialize all variables
        int index = (int)entry[0];
        string name = (string)entry[1];
        string accid = (string)entry[2];

        //Build list
        List<string> entryList = new List<string>();
        entryList.Insert(0, name); //0
        entryList.Insert(1, accid); //1

        //Create dictionary
        _profiles.Add(index, entryList);
    }

    public void OnDropdownChanged()
    {
        //Find Integer the dropdown is using
        int chosenInt = optionsCount+1 - _accIDDropdown.value;
        //If it isn't the starting int and dropdown is enabled, set account ID to it.
        if (chosenInt != optionsCount + 1 && StaticData.isUsingProfiles)
        {
            StaticData.CurrentAccountID = _profiles[chosenInt][1];
            print("USING PROFILE ID:  " + StaticData.CurrentAccountID);
        }
        
    }
}


