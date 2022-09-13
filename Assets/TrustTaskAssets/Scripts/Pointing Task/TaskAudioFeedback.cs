using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskAudioFeedback : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip SuccessClip;
    public AudioClip FailureClip;
    public AudioClip ConfirmationClip;
    public AudioClip DoneClip;
    public AudioClip FinishClip;
    public AudioClip NextClip;
    public AudioClip BeginClip;

    public void Setup(FoeIdentificationRunner taskRunner, ExperimentRunner experimentRunner)
    {
        // taskRunner.TargetHit += (sender, args) => { PlaySuccess(); };
        // taskRunner.TargetMissed += (sender, args) => { PlayFailure(); };
        // experimentRunner.StateChanged += PlayStateTransition;
    }

    private void PlayStateTransition(object sender, ExperimentState args)
    {
        switch(args)
        {
            case ExperimentState.NOT_STARTED:
                PlayBegin();
                break;
            case ExperimentState.PRE_TRAINING_TUTORIAL:
                PlayNext();
                break;
            case ExperimentState.TRAINING_TUTORIAL:
                PlayNext();
                break;
            case ExperimentState.POST_TRAINING_TUTORIAL:
                break;
            case ExperimentState.PRE_TEST_TUTORIAL:
                PlayNext();
                break;
            case ExperimentState.TEST_TUTORIAL:
                PlayNext();
                break;
            case ExperimentState.POST_TEST_TUTORIAL:
                break;  
            case ExperimentState.PRE_TRAINING:
                PlayNext();
                break;
            case ExperimentState.TRAINING_TASK:
                PlayNext();
                break;
            case ExperimentState.POST_TRAINING:
                break;                
            case ExperimentState.PRE_TEST:
                PlayNext();
                break;
            case ExperimentState.POINTING_TEST:
                PlayNext();
                break;
            case ExperimentState.POST_TEST:
                break;
            case ExperimentState.FINISHED:
                PlayFinish();
                break;   
        }
    }

    private void PlaySuccess()
    {
        AudioSource.PlayOneShot(SuccessClip);
    }

    private void PlayFailure()
    {
        AudioSource.PlayOneShot(FailureClip);
    }

    private void PlayConfirmation()
    {
        AudioSource.PlayOneShot(ConfirmationClip);
    }

    private void PlayDone()
    {
        AudioSource.PlayOneShot(DoneClip);
    }

    private void PlayFinish()
    {
        AudioSource.PlayOneShot(FinishClip);
    }

    private void PlayNext()
    {
        AudioSource.PlayOneShot(NextClip);
    }

    private void PlayBegin()
    {
        AudioSource.PlayOneShot(BeginClip);
    }
}
