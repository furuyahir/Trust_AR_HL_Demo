using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingTaskTarget : MonoBehaviour
{
    public bool IsEnemy;
    public bool TargetIsEnemy()
    {
        return IsEnemy;
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
