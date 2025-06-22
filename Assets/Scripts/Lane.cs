using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;


public class Lane : MonoBehaviour
{
    [SerializeField] private SplineContainer forwardSpline;
    [SerializeField] private SplineContainer backwardSpline;
    
    [Tooltip("The prefab to spawn as a note. Must have a Note component attached.")]
    [SerializeField] private GameObject notePrefab;
    
    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnNote(1);
        }
        
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SpawnNote(1, true);
        }
#endif
        
    }

    // Call this function to spawn a note.
    // Duration means how long the note takes to go from one end to the other
    // Reverse means it starts from the other direction (the reverse spline)
    void SpawnNote(float duration, bool reverse = false)
    {
        var targetContainer = reverse ? forwardSpline : backwardSpline;
        
        var note = Instantiate(notePrefab, targetContainer.Spline.EvaluatePosition(0), targetContainer.transform.rotation);
        var splineAnimate = note.GetComponent<SplineAnimate>();
        splineAnimate.Container = targetContainer;
        splineAnimate.Duration = duration;
        splineAnimate.Play();
    }
}