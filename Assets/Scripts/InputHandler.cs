using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    Toggle _toggleWhole;

    [SerializeField]
    Toggle _toggleUseFileName;

    [SerializeField]
    Toggle _toggleUsingProfiles;
    [SerializeField]
    Toggle _toggleisWritingFileNameAsACCID;
    [SerializeField]
    Toggle _toggleisWritingFileNameAsProfileID;


    [SerializeField]
    TMP_Text _brokerText;
    [SerializeField]
    TMP_Text _accText;
    [SerializeField]
    TMP_InputField _accField;

    [SerializeField]
    TMP_InputField _fileField;

    [SerializeField]
    TMP_Dropdown _profileDropdown;

    [SerializeField]
    Button _buttonStart;


    public void UpdateCheckBox()
    {
        StaticData.isRemovingWhole = _toggleWhole.isOn;
        StaticData.isUsingFileName = _toggleUseFileName.isOn;
        StaticData.isUsingProfiles = _toggleUsingProfiles.isOn;
        StaticData.isWritingFileNameAsACCID = _toggleisWritingFileNameAsACCID.isOn;
        StaticData.isWritingFileNameAsProfileID = _toggleisWritingFileNameAsProfileID.isOn;

        if (StaticData.isUsingFileName)
        {
            _toggleUsingProfiles.interactable = false;
            _profileDropdown.interactable = false;
            _accField.interactable = false;
            
        }
        else if (StaticData.isUsingProfiles == true)
            {
            _profileDropdown.interactable = true;
            _toggleUseFileName.interactable = false;
            _accField.interactable = false;
            _buttonStart.interactable = false;
        }
        else 
        {
            _toggleUseFileName.interactable = true;
            _toggleUsingProfiles.interactable = true;
            _profileDropdown.interactable = false;
            _accField.interactable = true;
        }

        if (StaticData.isWritingFileNameAsACCID)
        {
            _fileField.interactable = false;
            _toggleisWritingFileNameAsProfileID.interactable = false;

        }
        else if (StaticData.isWritingFileNameAsProfileID)
        {
            _fileField.interactable = false;
            _toggleisWritingFileNameAsACCID.interactable = false;
        }
        else
        {
            _fileField.interactable = true;
            _toggleisWritingFileNameAsProfileID.interactable = true;
            _toggleisWritingFileNameAsACCID.interactable = true;
        }


    }

    public void UpdateInputField()
    {
        //Update vars based on text field input
        if (!string.IsNullOrEmpty(_fileField.text) && _fileField.interactable)
        {
            StaticData.OutputName = _fileField.text;
        }

        StaticData.CurrentBrokerID = _brokerText.text;
        StaticData.CurrentAccountID = _accText.text;
        

    }
}
