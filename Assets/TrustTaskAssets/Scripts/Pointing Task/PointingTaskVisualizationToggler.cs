using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingTaskVisualizationToggler : MonoBehaviour
{
    private FoeIdentificationRunner FoeIdentificationRunner;
    private GameObject Crosshair;
    private Dictionary<PointingTaskTarget, string> TargetNames;

    private FOVConditions FOVConditionSetter;
    private AEPostProcess ResolutionConditionSetter;

    public void SetArguments(PointingTaskSetupArgs args)
    {
        UnsubscribeAll();
        TurnOffVisualizations();
        
        FoeIdentificationRunner = args.FoeIdentificationRunner;
        Crosshair = args.Crosshair;
        FOVConditionSetter = args.FOVConditionSetter;
        ResolutionConditionSetter = args.ResolutionConditionSetter;
        FoeIdentificationRunner.TrialStarted += FoeIdentificationRunnerOnTrialStarted;
        FoeIdentificationRunner.TaskStarted += FoeIdentificationRunnerOnTaskStarted;
        FoeIdentificationRunner.PointingTrainingRunnerTurnedOff += FoeIdentificationRunnerOnTrainingFinished;
    }

    private void UnsubscribeAll()
    {
        if (FoeIdentificationRunner == null)
        {
            return;
        }

        FoeIdentificationRunner.TrialStarted -=
            FoeIdentificationRunnerOnTrialStarted;
        FoeIdentificationRunner.PointingTrainingRunnerTurnedOff -= FoeIdentificationRunnerOnTrainingFinished;
    }

    private void FoeIdentificationRunnerOnTaskStarted(object sender, FoeIdentificationEventArgs args)
    {
        Crosshair.SetActive(true);

        TurnOffVisualizations();
        switch (args.ARCondition)
        {
            default:
                break;
        }
    }

    private void FoeIdentificationRunnerOnTrainingFinished(object sender, EventArgs e)
    {
        Crosshair.SetActive(false);
        TurnOffVisualizations();
    }

    // Start is called before the first frame update
    void Start()
    {
        TurnOffVisualizations();
    }

    private void FoeIdentificationRunnerOnTrialStarted(object sender, FoeIdentificationEventArgs e)
    {
        TurnOffVisualizations();
        ARCondition a = e.ARCondition;
        FOVConditionSetter.HFOV = a.HFOV;
        FOVConditionSetter.VFOV = a.VFOV;
        ResolutionConditionSetter.mipLevel = a.mipLevel;
        ResolutionConditionSetter.xOffset = a.xOffset;
        ResolutionConditionSetter.yOffset = a.yOffset;
        ResolutionConditionSetter.zOffset = a.zOffset;
        ResolutionConditionSetter.xOrient = a.xOrient;
        ResolutionConditionSetter.yOrient = a.yOrient;
        ResolutionConditionSetter.zOrient = a.zOrient;
        ResolutionConditionSetter.opacityLevel = a.opacityLevel;
    }

    private void TurnOffVisualizations()
    {
    }
}
