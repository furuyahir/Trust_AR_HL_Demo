using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class FoeIdentificationRunner : MonoBehaviour
{
    private TargetGenerator TargetGenerator;
    
    private PointingTaskConfig Config;
    private List<GameObject> RaycastDirectionGameObjects;

    public event EventHandler<FoeIdentificationEventArgs> TaskStarted;
    public event EventHandler<FoeIdentificationEventArgs> TrialFinalized;
    public event EventHandler<FoeIdentificationEventArgs> TrialStarted;
    public event EventHandler TaskFinished;
    public event EventHandler PointingTrainingRunnerTurnedOff;

    public event EventHandler<FoeIdentificationEventArgs> TargetHit;
    
    private PointingTaskTarget currentTarget;
    private int idx;
    private HashSet<PointingTaskTarget> TargetSet;
    private HashSet<PointingTaskTarget> TargetsHit;

    private int numTrials;

    public void SetArguments(PointingTaskSetupArgs args)
    {
        Config = args.Config;
        RaycastDirectionGameObjects = args.RaycastDirectionGameObjects;
        TargetGenerator = args.TargetGenerator;
    }

    public void StartTask()
    {
        SetIdx(0);
        TaskStarted?.Invoke(this, new FoeIdentificationEventArgs(null, null, TargetsHit));
    }

    public void TurnOff()
    {
        foreach (var target in TargetSet)
        {
            target.Hide();
        }
        PointingTrainingRunnerTurnedOff?.Invoke(this, EventArgs.Empty);
    }

    public void TryHit()
    {
        AnyTryHit();
    }

    public void TryNext()
    {
        IncrementIdx();
    }

    private void SetIdx(int newIdx)
    {
        idx = newIdx;
        List<PointingTaskTarget> targets = new List<PointingTaskTarget>();
        List<PointingTaskTarget> friends = new List<PointingTaskTarget>();
        TargetGenerator.GenerateTargets(Config.NumEnemies, Config.NumFriends, ref targets, ref friends);
        targets.AddRange(friends);
        TargetSet = new HashSet<PointingTaskTarget>(targets);
        TargetsHit = new HashSet<PointingTaskTarget>();
        
        TrialStarted?.Invoke(this, new FoeIdentificationEventArgs(null, Config.Conditions[idx], TargetSet));
    }

    private void FinalizeTrial()
    {
        TrialFinalized?.Invoke(this, new FoeIdentificationEventArgs(null, Config.Conditions[idx], null, "", TargetsHit.Count));
    }

    private void AnyTryHit()
    {
        if (TargetsHit == null)
        {
            Debug.LogWarning("Try to start new task.");
            return;
        }
        
        PointingTaskTarget target;
        if (IsAnyCurrentTarget(RaycastPotentialTargets(), out target))
        {
            ConfirmHit(target);
            TargetsHit.Add(target);
        }
    }

    private void IncrementIdx()
    {
        FinalizeTrial();
        if (idx >= Config.Conditions.Count - 1)
        {
            FinishTask();
            return;
        }
        
        SetIdx(idx + 1);
    }

    private bool IsAnyCurrentTarget(GameObject comparisonTarget, out PointingTaskTarget hitTarget)
    {
        hitTarget = null;
        if (comparisonTarget == null)
        {
            Debug.Log("Null");
            return false;
        }

        PointingTaskTarget t = comparisonTarget.GetComponent<PointingTaskTarget>();
        if (t == null)
        {
            Debug.LogWarning("Target does not have component PointingTaskTarget.");
            return false;
        }

        foreach(var target in TargetSet)
        {
            if (t == target)
            {
                hitTarget = target;
                return true;
            }
        }
        
        Debug.Log("Not a target");

        return false;
    }

    private void ConfirmHit(PointingTaskTarget target)
    {
        Debug.Log("Target hit.");
        TargetHit?.Invoke(this, new FoeIdentificationEventArgs(target, Config.Conditions[idx], null, "", target.TargetIsEnemy()? 1 : 0));
    }

    private void FinishTask()
    {
        Debug.LogWarning("Task done");
        TaskFinished?.Invoke(this, EventArgs.Empty);
    }

    private GameObject RaycastPotentialTargets()
    {
        RaycastHit hitInfo;
        Vector3 direction;
        int layer = 1 << LayerMask.NameToLayer("PointingTaskTargetLayer");
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, Mathf.Infinity, layer))
        {
            return hitInfo.collider.gameObject;
        }
        
        foreach (GameObject raycastDirectionGameObject in RaycastDirectionGameObjects)
        {
            direction = raycastDirectionGameObject.transform.position - Camera.main.transform.position;
            if (Physics.Raycast(Camera.main.transform.position, direction, out hitInfo, Mathf.Infinity, layer))
            {
                return hitInfo.collider.gameObject;
            }
        }

        Debug.LogWarning("No pointing task target was hit.");

        return null;
    }
}

public class FoeIdentificationEventArgs : EventArgs
{
    public PointingTaskTarget PointingTaskTarget;
    public ARCondition ARCondition;
    public HashSet<PointingTaskTarget> TargetSet;
    public string Name;
    public float Score;

    public FoeIdentificationEventArgs(PointingTaskTarget pointingTaskTarget, ARCondition arCondition, HashSet<PointingTaskTarget> targetSet, string name = "", float score = 0)
    {
        PointingTaskTarget = pointingTaskTarget;
        ARCondition = arCondition;
        TargetSet = targetSet;
        Name = name;
        Score = score;
    }
}