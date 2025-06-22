using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;


public class Lane : MonoBehaviour
{
    [Tooltip("The spline to use for animating notes.")]
    [SerializeField] private SplineContainer spline;
    
    [Tooltip("The prefab to spawn as a note. Must have a Note component attached.")]
    [SerializeField] private GameObject notePrefab;
    
    List<Note> _notes = new List<Note>();

    [Tooltip("The key that triggers a hit on the note")]
    [SerializeField] private InputAction key;

    private void OnEnable()
    {
        key.Enable();
    }

    private void Awake()
    {
        key.performed += Hit;
    }

    private void OnDisable()
    {
        key.Disable();
        
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnNote(5);
        }
#endif
        
    }

    void Hit(InputAction.CallbackContext context)
    {
        var note = _notes[0];
        _notes.RemoveAt(0);
        note.OnNoteHit?.Invoke();
        
    }

    // Call this function to spawn a note.
    // Duration means how long the note takes to go from one end to the other
    // Reverse means it starts from the other direction (the reverse spline)
    void SpawnNote(float duration)
    {
        var noteGameObject = Instantiate(notePrefab, spline.Spline.EvaluatePosition(0), spline.transform.rotation);
        
        var note =  noteGameObject.GetComponent<Note>();
        _notes.Add(note);
        
        var splineAnimate = noteGameObject.GetComponent<SplineAnimate>();
        splineAnimate.Container = spline;
        splineAnimate.Duration = duration;
        
        splineAnimate.Play();

        
    }
}