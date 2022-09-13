using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.WSA;
using Random = UnityEngine.Random;

public class LoggingController : MonoBehaviour
{
    public int ParticipantID;
    public bool LogData;
    public ExperimentRunner ExperimentRunner;
    public FoeIdentificationRunner foeIdentificationRunner;
    private DirectoryInfo FolderInfo;

    private ExperimentLogger ExperimentLogger;
    
    private void Awake()
    {
        if (!LogData)
        {
            return;
        }

        ExperimentLogger = GetComponent<ExperimentLogger>();
        
        FolderInfo = CreateNewFolder();

        ExperimentLogger.Init(FolderInfo, ExperimentRunner, foeIdentificationRunner, ParticipantID);
        // TransformLogger.Init(FolderInfo, PointingTrainingRunner, ParticipantID);
    }
    
    public DirectoryInfo CreateNewFolder()
    {
        string datePrefix = "Participant_" + ParticipantID + "_" + DateTime.Now.ToString("MMddyyyy-HHmm");
        return Directory.CreateDirectory(Path.Combine("Results", datePrefix));
    }
}
