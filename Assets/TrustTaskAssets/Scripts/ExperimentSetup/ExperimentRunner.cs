using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExperimentRunner : MonoBehaviour
{
    private PointingTaskVisualizationToggler PointingTaskVisualizationToggler;
    private FoeIdentificationRunner FoeIdentificationRunner;

    private PointingTaskSetupArgs TutorialArgs;
    private PointingTaskSetupArgs TrainingArgs;


    public InputAction FireAction;
    public InputAction FinalizeAction;
    public InputAction BackAction;
    private Action UIAction;
    private Action ControllerAction;

    private ExperimentState _experimentState;

    private Action<InputAction.CallbackContext> FireDelegate;
    private Action<InputAction.CallbackContext> FinalizeDelegate;
    private EventHandler StepHandler;

    
    public event EventHandler<ExperimentState> StateChanged; 


    public void SetArguments(PointingTaskVisualizationToggler pointingTaskVisualizationToggler,
        FoeIdentificationRunner foeIdentificationRunner,  PointingTaskSetupArgs tutorialArgs, PointingTaskSetupArgs trainingArgs)
    {
        PointingTaskVisualizationToggler = pointingTaskVisualizationToggler;
        FoeIdentificationRunner = foeIdentificationRunner;
        TutorialArgs = tutorialArgs;
        TrainingArgs = trainingArgs;
        GoNotStarted();
        FireDelegate = delegate(InputAction.CallbackContext context) { FoeIdentificationRunner.TryHit(); Debug.Log("Fire");};
        FinalizeDelegate = delegate(InputAction.CallbackContext context) { FoeIdentificationRunner.TryNext(); Debug.Log("Space");};
        StepHandler = delegate(object sender, EventArgs args)
        {
            Step();
        };
        StateChanged += delegate(object sender, ExperimentState state) { Debug.Log($"State = {state}"); };
    }
    
    public void Step()
    {
        switch (_experimentState)
        {
            case ExperimentState.NOT_STARTED:
                GoPreTrainingTutorial();
                break;
            case ExperimentState.PRE_TRAINING_TUTORIAL:
                GoTrainingTutorial();
                break;
            case ExperimentState.TRAINING_TUTORIAL:
                GoPostTrainingTutorial();
                break;
            case ExperimentState.POST_TRAINING_TUTORIAL:
                // GoPreTestTutorial();
                GoFinish();
                break;
            case ExperimentState.PRE_TEST_TUTORIAL:
                GoTestTutorial();
                break;
            case ExperimentState.TEST_TUTORIAL:
                GoPostTestTutorial();
                break;
            case ExperimentState.POST_TEST_TUTORIAL:
                GoPreTraining();
                break;
            case ExperimentState.PRE_TRAINING:
                GoTrainingTask();
                break;
            case ExperimentState.TRAINING_TASK:
                GoPostTraining();
                break;
            case ExperimentState.POST_TRAINING:
                GoPreTest();
                break;                
            case ExperimentState.PRE_TEST:
                GoPointingTest();
                break;
            case ExperimentState.POINTING_TEST:
                GoPostTest();
                break;
            case ExperimentState.POST_TEST:
                GoFinish();
                break;
            case ExperimentState.FINISHED:
                break;
        }
        StateChanged?.Invoke(this, _experimentState);
    }

    private void OnEnable()
    {
        FireAction.Enable();
        FinalizeAction.Enable();
        BackAction.Enable();
    }
    
    private void OnDisable()
    {
        FireAction.Disable();
        FinalizeAction.Disable();
        BackAction.Disable();
    }

    private Action<InputAction.CallbackContext> UIGo;
    // Need trigger for: go tutorial
    //                   go training
    //                   go test 
    //                   go finish
    private void GoNotStarted()
    {
        _experimentState = ExperimentState.NOT_STARTED;
        UIGo = delegate (InputAction.CallbackContext context)
        { 
            Step(); 
        };
        UIAction += () => Step();
        FireAction.performed += UIGo;
        StateChanged?.Invoke(this, _experimentState);
    }

    private void GoConditionSelect()
    {
        FireAction.performed -= UIGo;
        _experimentState = ExperimentState.PRE_TRAINING_TUTORIAL;
    }
    
    
    private void GoPreTrainingTutorial()
    {
        _experimentState = ExperimentState.PRE_TRAINING_TUTORIAL;
        PointingTaskVisualizationToggler.SetArguments(TutorialArgs);
        FoeIdentificationRunner.SetArguments(TutorialArgs);
        FoeIdentificationRunner.StartTask();
    }

    private void GoTrainingTutorial()
    {
        _experimentState = ExperimentState.TRAINING_TUTORIAL;
        FireAction.performed -= UIGo;


        FoeIdentificationRunner.TaskFinished += StepHandler;
        
        FireAction.performed += FireDelegate;
        FinalizeAction.performed += FinalizeDelegate;

    }

    private void GoPostTrainingTutorial()
    {
        _experimentState = ExperimentState.POST_TRAINING_TUTORIAL;
        
        FoeIdentificationRunner.TaskFinished -= StepHandler;
        FireAction.performed -= FireDelegate;
        FinalizeAction.performed -= FinalizeDelegate;
        
        FireAction.performed += UIGo;
    }

    private void GoPreTestTutorial()
    {
        FoeIdentificationRunner.TurnOff();
        _experimentState = ExperimentState.PRE_TEST_TUTORIAL;

    }

    private Action<InputAction.CallbackContext> TryBack;

    private void GoTestTutorial()
    {
        _experimentState = ExperimentState.TEST_TUTORIAL;
        FireAction.performed -= UIGo;


        
        TestTryNext = delegate(InputAction.CallbackContext context) 
        {
        };
        TryBack = delegate(InputAction.CallbackContext context) 
        { 
        };
        FireAction.performed += TestTryNext;
        BackAction.performed += TryBack;

        TestForceNext = delegate() { };
        ControllerAction += TestForceNext;
    }

    private void GoPostTestTutorial()
    {
        _experimentState = ExperimentState.POST_TEST_TUTORIAL;
        FireAction.performed -= TestTryNext;
        BackAction.performed -= TryBack;
        ControllerAction -= TestForceNext;

        FireAction.performed += UIGo;
    }
    
    private void GoPreTraining()
    {
        _experimentState = ExperimentState.PRE_TRAINING;
        PointingTaskVisualizationToggler.SetArguments(TrainingArgs);
        FoeIdentificationRunner.TurnOff();
    }

    private void GoTrainingTask()
    {
        FireAction.performed -= UIGo;
        
        _experimentState = ExperimentState.TRAINING_TASK;
        FoeIdentificationRunner.SetArguments(TrainingArgs);
        FoeIdentificationRunner.StartTask();
        
        FoeIdentificationRunner.TaskFinished += StepHandler;
        FireAction.performed += FireDelegate;
    }

    private void GoPostTraining()
    {
        _experimentState = ExperimentState.POST_TRAINING;
        FoeIdentificationRunner.TaskFinished -= StepHandler;
        FireAction.performed -= FireDelegate;

        FireAction.performed += UIGo;        
    }

    private void GoPreTest()
    {
        FoeIdentificationRunner.TurnOff();
        _experimentState = ExperimentState.PRE_TEST;
    }
    
    private Action<InputAction.CallbackContext> TestTryNext;
    private Action TestForceNext;

    private void GoPointingTest()
    {
        _experimentState = ExperimentState.POINTING_TEST;

        FireAction.performed -= UIGo; 

        
        TestTryNext = delegate(InputAction.CallbackContext context) 
        { 
        }; 

        TestForceNext = delegate() {  };
        FireAction.performed += TestTryNext;
        ControllerAction += TestForceNext;
        BackAction.performed += TryBack;
    }

    private void GoPostTest()
    {
        FireAction.performed -= TestTryNext;
        ControllerAction -= TestForceNext;
        BackAction.performed -= TryBack;

        FireAction.performed += UIGo;

        _experimentState = ExperimentState.POST_TEST;
    }
    
    private void GoFinish()
    {
        _experimentState = ExperimentState.FINISHED;
        FireAction.performed -= UIGo;
        Debug.Log("Experiment finished.");
    }
}

public enum ExperimentState
{
    NOT_STARTED,
    PRE_TRAINING_TUTORIAL,
    TRAINING_TUTORIAL,
    POST_TRAINING_TUTORIAL,
    PRE_TEST_TUTORIAL,
    TEST_TUTORIAL,
    POST_TEST_TUTORIAL,
    PRE_TRAINING,
    TRAINING_TASK,
    POST_TRAINING,
    PRE_TEST,
    POINTING_TEST,
    POST_TEST,
    FINISHED
}


public enum TechniqueVariable
{
    NOTHING
}