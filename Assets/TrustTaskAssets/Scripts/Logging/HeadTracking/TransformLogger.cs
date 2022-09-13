using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TransformLogger : MonoBehaviour
{
    private int ParticipantID;
    private List<TransformLog> CurrLogs;
    private TechniqueVariable currTechniqueVariable;
    private Transform PedestrianHeadTransform;
    private string DirectoryPath;
    private Coroutine LoggerCoroutine;
    
    public void Init(DirectoryInfo folderInfo, FoeIdentificationRunner p, int participantId)
    {
        ParticipantID = participantId;
        p.TaskStarted += TaskStartedHandler;
        p.TaskFinished += TaskEndedHandler;
        PedestrianHeadTransform = Camera.main.transform;
        CurrLogs = new List<TransformLog>();
        CreateFolder(folderInfo);
    }

    private void CreateFolder(DirectoryInfo folderInfo)
    {
        DirectoryPath = Directory.CreateDirectory(Path.Combine(folderInfo.FullName, "TransformLogging")).FullName;
    }

    private IEnumerator LogTransforms()
    {
        while (true)
        {
            TransformLog newLog = new TransformLog()
            {
                Timestamp = TimeMillis(),
                HeadPosition = PedestrianHeadTransform.position,
                HeadRotation = PedestrianHeadTransform.rotation,
            };
            CurrLogs.Add(newLog);
            yield return null;
        }
    }

    private void WriteFile()
    {
        string filePrefix = $"P{ParticipantID}";
        string filePath = Path.Combine(DirectoryPath, filePrefix + "_transforms.csv");
        StreamWriter writerRaw = new StreamWriter(filePath, false);

        writerRaw.WriteLine("Condition");
        writerRaw.WriteLine($"{(int)currTechniqueVariable}");
        
        string header =
            "ts, HeadPosX, HeadPosY, HeadPosZ, HeadRotW, HeadRotX, HeadRotY, HeadRotZ";
        

        writerRaw.WriteLine(header);
        for (int i = 0; i < CurrLogs.Count; i++)
        {
            writerRaw.WriteLine(CurrLogs[i].ToString());
        }
        writerRaw.Close();
    }

    public void TaskStartedHandler(object sender, FoeIdentificationEventArgs args)
    {
        CurrLogs = new List<TransformLog>();
        StartLogging();
    }

    private void StartLogging()
    {
        if (LoggerCoroutine != null)
        {
            StopCoroutine(LoggerCoroutine);
        }
        LoggerCoroutine = StartCoroutine(LogTransforms());        
    }

    public void TaskEndedHandler(object sender, EventArgs args)
    {
        WriteFile();
    }

    private long TimeMillis()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}


public class TransformLog
{
    public long Timestamp;
    public Vector3 HeadPosition;
    public Quaternion HeadRotation;

    public override string ToString()
    {
        return 
            $"{Timestamp}, " +
            $"{HeadPosition.x}, {HeadPosition.y}, {HeadPosition.z}, " +
            $"{HeadRotation.w}, {HeadRotation.x}, {HeadRotation.y}, {HeadRotation.z}";
    }
}

