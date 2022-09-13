using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingTaskSetup : MonoBehaviour
{
    public ExperimentRunner ExperimentRunner;
    public PointingTaskConfig TestConfig;
    public PointingTaskConfig TutorialConfig;
    public TaskAudioFeedback TaskAudioFeedback;
    public List<GameObject> RaycastDirectionGameObjects;
    public GameObject Crosshair;
    public FOVConditions FOVConditionSetter;
    public AEPostProcess ResolutionConditionSetter;
    
    public FoeIdentificationRunner foeIdentificationRunner;
    public PointingTaskVisualizationToggler PointingTaskVisualizationToggler;
    public TargetGenerator TargetGenerator;


    public PointingTaskSetupArgs GetTutorialSetup()
    {
        PointingTaskSetupArgs args = new PointingTaskSetupArgs();
        args.Config = TutorialConfig;
        args.FoeIdentificationRunner = foeIdentificationRunner;
        args.RaycastDirectionGameObjects = RaycastDirectionGameObjects;
        args.Crosshair = Crosshair;
        args.TargetGenerator = TargetGenerator;
        args.FOVConditionSetter = FOVConditionSetter;
        args.ResolutionConditionSetter = ResolutionConditionSetter;
        return args;
    }
    
    public PointingTaskSetupArgs GetTestSetup()
    {
        PointingTaskSetupArgs args = new PointingTaskSetupArgs();
        args.Config = TestConfig;
        args.FoeIdentificationRunner = foeIdentificationRunner;
        args.RaycastDirectionGameObjects = RaycastDirectionGameObjects;
        args.Crosshair = Crosshair;
        args.TargetGenerator = TargetGenerator;
        args.FOVConditionSetter = FOVConditionSetter;
        args.ResolutionConditionSetter = ResolutionConditionSetter;
        return args;
    }

    
    private void Awake()
    {
        TaskAudioFeedback.Setup(foeIdentificationRunner, ExperimentRunner);
        ExperimentRunner.SetArguments(PointingTaskVisualizationToggler, foeIdentificationRunner, GetTutorialSetup(), GetTestSetup());

        Crosshair.SetActive(false);
    }
}

public class PointingTaskSetupArgs
{
    public PointingTaskConfig Config;
    public FoeIdentificationRunner FoeIdentificationRunner;
    public TargetGenerator TargetGenerator;
    public List<GameObject> RaycastDirectionGameObjects;
    public GameObject Crosshair;
    public FOVConditions FOVConditionSetter;
    public AEPostProcess ResolutionConditionSetter;
}