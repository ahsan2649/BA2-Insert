using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineAnimate))]
public class Note : MonoBehaviour
{
    private SplineAnimate _splineAnimate;
    public Action OnNoteHit;
    
    
    void Awake()
    {
        _splineAnimate = GetComponent<SplineAnimate>();
        _splineAnimate.Completed += SplineAnimateOnCompleted;
        OnNoteHit += SplineAnimateOnCompleted;
    }
    
    void SplineAnimateOnCompleted()
    {
        _splineAnimate.Completed -= SplineAnimateOnCompleted;
        OnNoteHit -= SplineAnimateOnCompleted;
        Destroy(gameObject);
    }
    
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
