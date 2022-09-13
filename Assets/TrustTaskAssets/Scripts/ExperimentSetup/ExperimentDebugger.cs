using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentDebugger : MonoBehaviour
{
    public FoeIdentificationRunner FoeIdentificationRunner;

    private void Start()
    {
        FoeIdentificationRunner.TaskStarted += (sender, args) => Debug.Log("Task started");
        FoeIdentificationRunner.TrialStarted += (sender, args) => Debug.Log("Trial started");
        FoeIdentificationRunner.TargetHit += (sender, args) => Debug.Log($"Target hit: {args.Score == 1}");
        FoeIdentificationRunner.TrialFinalized += (sender, args) => Debug.Log("Trial finalized");
        FoeIdentificationRunner.TaskFinished += (sender, args) => Debug.Log("Task finished");
    }
}
