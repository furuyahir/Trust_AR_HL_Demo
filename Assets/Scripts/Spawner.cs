using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Spawner : TargetGenerator
{
    public List<PointingTaskTarget> Enemies;
    public List<PointingTaskTarget> Friends;
    public Transform[] spawnPoints;

    private List<PointingTaskTarget> spawnedTargets;
  
    public override void GenerateTargets(int totalEnemies, int totalFriends, ref List<PointingTaskTarget> enemies, ref List<PointingTaskTarget> friends)
    {
        if (spawnPoints.Length < (Enemies.Count + Friends.Count))
        {
            Debug.LogError("Need more spawn points");
            return;
        }

        DestroyCurrentTargets();
        spawnedTargets = new List<PointingTaskTarget>();

        enemies = new List<PointingTaskTarget>();
        friends = new List<PointingTaskTarget>();
        var shuffledSpawnPoints = spawnPoints.OrderBy(a => Guid.NewGuid()).ToList();
        int idx = 0;

        foreach (var enemyPrefab in Enemies)
        {
            PointingTaskTarget t = Instantiate(enemyPrefab, shuffledSpawnPoints[idx].position, shuffledSpawnPoints[idx].rotation);
            enemies.Add(t);
            idx++;
        }

        foreach (var friendPrefab in Friends)
        {
            PointingTaskTarget t = Instantiate(friendPrefab, shuffledSpawnPoints[idx].position, shuffledSpawnPoints[idx].rotation);
            friends.Add(t);
            idx++;
        }
    }

    private void DestroyCurrentTargets()
    {
        if (spawnedTargets == null || spawnedTargets.Count == 0)
        {
            return;
        }

        foreach (var spawnedTarget in spawnedTargets) 
        {
            Destroy(spawnedTarget);
        }
        spawnedTargets = null;
    }

}