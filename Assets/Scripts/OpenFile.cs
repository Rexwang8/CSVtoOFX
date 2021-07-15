using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
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

    public void Click()
    {



        //Open file to find path
        //FileBrowser.ShowLoadDialog(null, null, FileBrowser.PickMode.Files, false, "C:\\", "example.csv", "Save As", "Save");
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
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
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

}


