using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class TrackSpawner : MonoBehaviour
{
    [Header("Track Prefabs")]
    public GameObject[] trackPrefabs;     
    public Transform spawnParent;

    [Header("Spawn Settings")]
    public Vector3 trackOffset = new Vector3(13.2f, 0f, 7.3f); 
    private int trackCount = 1;

    [Header("Spawn Sequenece")]
    public int[] spawnSequence;

    private int nextIndex = 0;
    private Queue<GameObject> spawnedTracks = new Queue<GameObject>();
    private int maxTracks = 4;
    private int? forcedNextPrefab = null;
    
    public (SplineContainer mainSpline, SplineContainer leverSpline) SpawnNextTrack()
    {
        if (spawnSequence == null || spawnSequence.Length == 0)
        {
            return (null, null);
        }

        int prefabIndex;

        if (forcedNextPrefab.HasValue)
        {
            prefabIndex = forcedNextPrefab.Value;
            forcedNextPrefab = null;
        }
        else
        {
            prefabIndex = spawnSequence[nextIndex];
            nextIndex = (nextIndex + 1) % spawnSequence.Length;
        }

        GameObject prefab = trackPrefabs[prefabIndex];

        Vector3 spawnPos = trackOffset * trackCount;
        Quaternion spawnRot = Quaternion.Euler(0f, 180f, 0f);

        GameObject newTrack = Instantiate(prefab, spawnPos, spawnRot, spawnParent);
        trackCount++;

        spawnedTracks.Enqueue(newTrack);

        if (spawnedTracks.Count > maxTracks)
        {
            GameObject oldTrack = spawnedTracks.Dequeue();
            Destroy(oldTrack);
        }

        SplineContainer main = newTrack.GetComponentInChildren<SplineContainer>(); //assume main spline is first in hierarchy
        SplineContainer lever = null;

        foreach (var spline in newTrack.GetComponentsInChildren<SplineContainer>())
        {
            if (spline != main)
            {
                lever = spline; // second spline is pulledLeverSpline
                break;
            }
        }

        return (main, lever);
    }

    public void ForceNextTrack(int prefabIndex)
    {
        forcedNextPrefab = prefabIndex;
    }
}
