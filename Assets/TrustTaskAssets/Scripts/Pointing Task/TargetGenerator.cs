using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGenerator : MonoBehaviour
{
    public virtual void GenerateTargets(int totalEnemies, int totalFriends, ref List<PointingTaskTarget> enemies, ref List<PointingTaskTarget> friends)
    {
        enemies = new List<PointingTaskTarget>();
        friends = new List<PointingTaskTarget>();
    }
}
