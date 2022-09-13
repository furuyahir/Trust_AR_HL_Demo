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
    public GameObject friendTag;
    public GameObject enemyTag;

    public bool avatarsOnHL = true;
    private bool previousSetting = true;
    private List<GameObject> virtualHumans;

    private List<PointingTaskTarget> spawnedTargets;

    // quick function to turn on avatars on the HL for alignment purposes
    public void SetHumanVisibility(bool visible)
    {
        foreach(var vh in virtualHumans)
        {
            vh.gameObject.SetActive(visible);
        }
    }

    // check the state of the human visibility toggle, update layers if needed
    private void Update()
    {
        if(previousSetting != avatarsOnHL)
        {
            SetHumanVisibility(avatarsOnHL);
            previousSetting = avatarsOnHL;
        }
    }

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
        virtualHumans = new List<GameObject>();
        Transform camT = Camera.main.transform;
        foreach (var enemyPrefab in Enemies)
        {
            PointingTaskTarget t = Instantiate(enemyPrefab, shuffledSpawnPoints[idx].position, shuffledSpawnPoints[idx].rotation);
            
            t.transform.LookAt(new Vector3(camT.position.x,t.transform.position.y,camT.position.z));
            enemies.Add(t);
            idx++;
            for (int i = 0; i < t.transform.childCount; i++)
            {
                if (t.transform.GetChild(i).name.Contains("bones"))
                {
                    virtualHumans.Add(t.transform.GetChild(1).gameObject);
                    Instantiate(enemyTag, t.transform);
                    break;
                }
            }
        }

        foreach (var friendPrefab in Friends)
        {
            PointingTaskTarget t = Instantiate(friendPrefab, shuffledSpawnPoints[idx].position, shuffledSpawnPoints[idx].rotation);
            t.transform.LookAt(new Vector3(camT.position.x, t.transform.position.y, camT.position.z));
            friends.Add(t);
            idx++;
            for (int i = 0; i < t.transform.childCount; i++)
            {
                if (t.transform.GetChild(i).name.Contains("bones"))
                {
                    virtualHumans.Add(t.transform.GetChild(1).gameObject);
                    Instantiate(friendTag, t.transform);
                    break;
                }
            }
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