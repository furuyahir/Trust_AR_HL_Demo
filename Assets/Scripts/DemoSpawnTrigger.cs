using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSpawnTrigger : MonoBehaviour
{
    void Start()
    {
        Spawner s = (Spawner)FindObjectOfType(typeof(Spawner));

        List<PointingTaskTarget> targets = new List<PointingTaskTarget>();
        List<PointingTaskTarget> friends = new List<PointingTaskTarget>();
        s.GenerateTargets(0, 0, ref targets, ref friends);    
    }

}
