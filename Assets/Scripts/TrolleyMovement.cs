using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine.InputSystem;
public class TrolleyMovement : MonoBehaviour 
{ 
    [Header("Paths")] 
    public SplineContainer mainSpline; 
    public SplineContainer pulledLeverSpline; 

    [Header("Movement")] 
    public float speed = 3f; 
    private float distance; 
    
    [Header("Junction")]
    [Range(0f, 1f)] public float junctionT = 0.45f;  //where other track starts
    [Range(0f, 1f)] public float rejoinT = 0.75f; //where other track ends
    private bool junctionPassed = false; 
    
    [Header("Track Spawner")] 
    public TrackSpawner trackSpawner;
    
    private TrolleyControls controls; 
    private Queue<SplineContainer> splineQuene = new Queue<SplineContainer>(); 
    private SplineContainer currentSpline; 
    private bool? takeOtherPath = null; 
    private bool spawnedNextTrack = false; 
    
    private void Awake() 
    { 
        controls = new TrolleyControls(); 
        controls.Gameplay.SwitchPath.performed += ctx => 
        { 
            if (takeOtherPath == null && Keyboard.current != null) 
            { 
                if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame) 
                { 
                    takeOtherPath = false; 
                    Debug.Log("Player chose MAIN path"); 
                } 
                else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame) 
                { 
                    takeOtherPath = true; 
                    Debug.Log("Player chose OPTIONAL path"); 
                } 
            } 
        }; 
    } 
    
    private void OnEnable() => controls.Gameplay.Enable(); 
    private void OnDisable() => controls.Gameplay.Disable(); 
    
    void Start() 
    { 
        currentSpline = mainSpline; 
        distance = 0f;
        EnqueueNextMainSpline();
    } 
    
    void Update() 
    { 
        if (currentSpline == null) return; 
        
        distance += speed * Time.deltaTime; 
        float t = distance / currentSpline.CalculateLength(); 

        //move torlley
        transform.position = currentSpline.EvaluatePosition(t); 
        transform.rotation = Quaternion.LookRotation(currentSpline.EvaluateTangent(t)); 
                
        //spawn next rail prefab
        if (currentSpline.CompareTag("MainSpline") && !spawnedNextTrack && trackSpawner != null) 
        { 
            spawnedNextTrack = true;
            EnqueueNextMainSpline();
        } 
        
        //check for junction
        if (!junctionPassed && takeOtherPath == true && currentSpline == mainSpline && t >= junctionT) 
        { 
            if (pulledLeverSpline != null)
            {
                splineQuene.Enqueue(pulledLeverSpline);
            }

            junctionPassed = true;
        } 
        
        //at end of current spline?
        if (t >= 1f) 
        { 
            if (splineQuene.Count > 0) 
            { 
                currentSpline = splineQuene.Dequeue(); 
                
                if (currentSpline.name == mainSpline.name && junctionPassed) 
                { 
                    distance = rejoinT * mainSpline.CalculateLength(); 
                } 
                else 
                { 
                    distance = 0f; 
                }

                takeOtherPath = null;
                junctionPassed = false;

                if (currentSpline.CompareTag("MainSpline"))
                {
                    spawnedNextTrack = false;
                }
            } 
        } 
    } 

    private void EnqueueNextMainSpline()
    {
        if (trackSpawner == null) return;

        var nextTrack = trackSpawner.SpawnNextTrack();
        if (nextTrack.mainSpline != null)
        {
            splineQuene.Enqueue(nextTrack.mainSpline);

            mainSpline = nextTrack.mainSpline;
            pulledLeverSpline = nextTrack.leverSpline;
        }
    }
}