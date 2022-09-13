using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraDisplaySwitcherCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TMP_Dropdown dropdown;

    private int CameraIdx;
    public event EventHandler<CameraDisplaySwitchedEventArgs> CameraDisplayAssignmentChanged;  
    
    public void Init(Camera camera, int cameraIdx, int displays)
    {
        CameraIdx = cameraIdx;
        title.text = $"{camera.name} ({cameraIdx + 1})";
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < displays; i++)
        {
            options.Add(new TMP_Dropdown.OptionData($"Display {i + 1}"));
        }

        dropdown.options = options;
        dropdown.value = camera.targetDisplay;
        Debug.Log($"Camera {camera.name}: {camera.targetDisplay}");
        dropdown.onValueChanged.AddListener(value =>
        {
            CameraDisplayAssignmentChanged?.Invoke(this, 
                new CameraDisplaySwitchedEventArgs(CameraIdx, value));
        });
    }
}

public class CameraDisplaySwitchedEventArgs : EventArgs
{
    public int CameraIdx;
    public int DisplayIdx;

    public CameraDisplaySwitchedEventArgs(int cameraIdx, int displayIdx)
    {
        CameraIdx = cameraIdx;
        DisplayIdx = displayIdx;
    }
}