using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraDisplaySwitcher : MonoBehaviour
{
    [SerializeField] private CameraDisplaySwitcherCell template;
    [SerializeField] private Transform cellParent;
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button LoadButton;


    private Camera[] cameras;
    
    // Start is called before the first frame update
    void Start()
    {
        CameraDisplaySave cameraDisplaySave = new CameraDisplaySave();
        cameraDisplaySave.CameraDisplayAssignmentsLoaded += AssignLoadedCameraDisplayAssignments;
        cameras = Camera.allCameras;
        int numberDisplays = Display.displays.Length;
        if (Application.isEditor)
        {
            numberDisplays = 8;
        }
        
        for (int i = 0; i < cameras.Length; i++)
        {
            CameraDisplaySwitcherCell newCell = Instantiate(template, cellParent);
            newCell.Init(cameras[i], i, numberDisplays);
            newCell.CameraDisplayAssignmentChanged += (sender, args) =>
            {
                cameras[args.CameraIdx].targetDisplay = args.DisplayIdx;
                cameraDisplaySave.AssignCameraDisplay(args.CameraIdx, args.DisplayIdx);
            };
            newCell.gameObject.SetActive(true);
        }
        
        SaveButton.onClick.AddListener(() => cameraDisplaySave.SaveData());
        LoadButton.onClick.AddListener(() => cameraDisplaySave.LoadData());
    }

    private void AssignLoadedCameraDisplayAssignments(object sender, Dictionary<int, int> cameraDisplayAssignments)
    {
        foreach (KeyValuePair<int,int> cameraDisplayAssignment in cameraDisplayAssignments)
        {
            cameras[cameraDisplayAssignment.Key].targetDisplay = cameraDisplayAssignment.Value;
        }
    }
}
