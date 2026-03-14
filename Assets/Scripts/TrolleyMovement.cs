using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;

public class TrolleyMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;

    [Header("References")]
    public TrackSpawner trackSpawner;
    public GameObject choicesButtons;

    private float distance;
    private Queue<SplineContainer> splineQueue = new Queue<SplineContainer>();
    private SplineContainer currentSpline;
    private bool spawnedNextTrack = false;

    void Start()
    {
        currentSpline = null;
        distance = 0f;
        EnqueueNextMainSpline();
        NextSpline();            
    }

    void Update()
    {
        if (currentSpline == null) return;

        distance += speed * Time.deltaTime;
        float t = distance / currentSpline.CalculateLength();

        transform.position = currentSpline.EvaluatePosition(t);
        transform.rotation = Quaternion.LookRotation(currentSpline.EvaluateTangent(t));

        if (!spawnedNextTrack && trackSpawner != null && t <= 0.01f)
        {
            spawnedNextTrack = true;
            EnqueueNextMainSpline();
        }

        if (t >= 1f)
        {
            NextSpline();
        }
    }

    private void NextSpline()
    {
        if (splineQueue.Count > 0)
        {
            currentSpline = splineQueue.Dequeue();
            distance = 0f;
            spawnedNextTrack = false;
        }
        else
        {
            currentSpline = null;
        }
    }

    private void EnqueueNextMainSpline()
    {
        if (trackSpawner == null) return;

        var nextTrack = trackSpawner.SpawnNextTrack();
        if (nextTrack.mainSpline != null)
        {
            splineQueue.Enqueue(nextTrack.mainSpline);
        }
    }
}