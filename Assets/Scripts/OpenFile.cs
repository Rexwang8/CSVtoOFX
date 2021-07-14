using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
        Path = EditorUtility.OpenFilePanel("Open CSV", "", "csv");
        _pathDisplay.text = Path;

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
