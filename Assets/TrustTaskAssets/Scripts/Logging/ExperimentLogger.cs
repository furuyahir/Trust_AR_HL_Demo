using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.WSA;
using System.Linq;

public class ExperimentLogger : MonoBehaviour
{
    private int ParticipantID;
    private FoeIdentificationRunner FoeIdentificationRunner;
    private LogContainer LogContainer;

    private TrialLog _currentTrialLog;

    private DirectoryInfo FolderInfo;
    private string logPath;
    private StreamWriter writerRaw;
    
    public void Init(DirectoryInfo folderInfo, ExperimentRunner experimentRunner, FoeIdentificationRunner foeIdentificationRunner, int participantId)
    {
        ParticipantID = participantId;
        FolderInfo = folderInfo;
        foeIdentificationRunner.TaskStarted += TaskStartedHandler;
        foeIdentificationRunner.TrialStarted += TrialStartedHandler;
        foeIdentificationRunner.TargetHit += TargetHitHandler;
        foeIdentificationRunner.TrialFinalized += TrialFinalizedHandler;
        foeIdentificationRunner.TaskFinished += TrainingEndedHandler;

        LogContainer = new LogContainer();

        experimentRunner.StateChanged += OnExperimentRunnerStateChanged;
        
        OpenLog();
    }

    private void OnExperimentRunnerStateChanged(object sender, ExperimentState state)
    {
        switch (state)
        {
            case ExperimentState.NOT_STARTED:
                break;
            case ExperimentState.PRE_TRAINING_TUTORIAL:
                break;
            case ExperimentState.TRAINING_TUTORIAL:
                break;
            case ExperimentState.POST_TRAINING_TUTORIAL:
                break;
            case ExperimentState.PRE_TEST_TUTORIAL:
                break;
            case ExperimentState.TEST_TUTORIAL:
                break;
            case ExperimentState.POST_TEST_TUTORIAL:
                break;
            case ExperimentState.PRE_TRAINING:
                break;
            case ExperimentState.TRAINING_TASK:
                break;
            case ExperimentState.POST_TRAINING:
                break;                
            case ExperimentState.PRE_TEST:
                break;
            case ExperimentState.POINTING_TEST:
                break;
            case ExperimentState.POST_TEST:
                break;
            case ExperimentState.FINISHED:
                CloseLog();
                break;
        }
    }

    private void OpenLog()
    {
        string filePrefix = $"Participant_{ParticipantID}_logs.csv";
        logPath = Path.Combine(FolderInfo.FullName, filePrefix);
        writerRaw = new StreamWriter(logPath, false);
        Debug.Log($"Writing logs to: {logPath}");
        string header = $"{"Participant ID: "}, {ParticipantID}\n";
        writerRaw.Write(header);
    }

    private void CloseLog()
    {
        writerRaw.Close();
    }

    private void WriteLog(string str)
    {
        writerRaw.Write(str);
    }

    public void TaskStartedHandler(object sender, FoeIdentificationEventArgs args)
    {
        TrainingLog newStartLog = new TrainingLog();
        newStartLog.TrialStartedTimestamp = TimeMillis();
        LogContainer.StartEndLog = newStartLog;
    }

    private void TrialStartedHandler(object sender, FoeIdentificationEventArgs args)
    {
        TrialLog newLog = new TrialLog();
        newLog.TrialStartedTimeStamp = TimeMillis();
        _currentTrialLog = newLog;
    }
    
    private void TrialFinalizedHandler(object sender, FoeIdentificationEventArgs args)
    {
        _currentTrialLog.TrialEndedTimestamp = TimeMillis();
        WriteLog(_currentTrialLog.ToString());
    }

    private void TargetHitHandler(object sender, FoeIdentificationEventArgs args)
    {
        _currentTrialLog.TotalScore += (int)args.Score;
        TrainingTargetHitLog newLog = new TrainingTargetHitLog(TimeMillis(), args.Score == 1);
        List<TrainingTargetHitLog> hitLogs = _currentTrialLog.HitLogs;
        if (hitLogs.Count > 0)
        {
            newLog.TimeTaken = newLog.TargetHitTimestamp - hitLogs[hitLogs.Count - 1].TargetHitTimestamp;
        }
        else
        {
            newLog.TimeTaken = newLog.TargetHitTimestamp - _currentTrialLog.TrialStartedTimeStamp;
        }
        _currentTrialLog.HitLogs.Add(newLog);
    }

    private void TrainingEndedHandler(object sender, EventArgs args)
    {
        TrainingLog log = LogContainer.StartEndLog;
        log.TrialEndedTimestamp = TimeMillis();
        log.TimeToComplete = log.TrialEndedTimestamp - log.TrialStartedTimestamp;
        WriteLog(log.ToString());
    }
    
    
    private long TimeMillis()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}

public class LogContainer
{
    public TrainingLog StartEndLog;
    public List<TrialLog> TrialLogs;

    public LogContainer()
    {
        TrialLogs = new List<TrialLog>();
    }
}

public class TrainingLog
{
    public int Condition;
    public long TrialStartedTimestamp;
    public long TrialEndedTimestamp;
    public long TimeToComplete;

    public override string ToString()
    {
        return string.Format("{0}, {1}, {2}, {3}",
            Condition,
            TrialStartedTimestamp,
            TrialEndedTimestamp,
            TimeToComplete);
    }
}

public class TrialLog
{
    public int TotalScore;
    public long TrialStartedTimeStamp;
    public long TrialEndedTimestamp;
    public List<TrainingTargetHitLog> HitLogs;

    public TrialLog()
    {
        HitLogs = new List<TrainingTargetHitLog>();
    }

    public override string ToString()
    {
        string str = "";
        str += $"Trial score, {TotalScore}, Time taken, {TrialEndedTimestamp - TrialStartedTimeStamp}\n";
        for (int i = 0; i < HitLogs.Count; i++)
        {
            TrainingTargetHitLog hitLog = HitLogs[i];
            str += $"{i}, {hitLog}\n";
        }
        return str;
    }
}

public class TrainingTargetHitLog
{
    public long TargetHitTimestamp;
    public long TimeTaken;
    public bool Correct;

    public TrainingTargetHitLog(long targetHitTimestamp, bool correct)
    {
        TargetHitTimestamp = targetHitTimestamp;
        Correct = correct;
    }

    public override string ToString()
    {
        return string.Format("{0}, {1}",
            TimeTaken,
            Correct);
    }
}
