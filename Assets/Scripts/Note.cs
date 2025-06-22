using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineAnimate))]
public class Note : MonoBehaviour
{
    SplineAnimate _splineAnimate;
    void Awake()
    {
        _splineAnimate = GetComponent<SplineAnimate>();
        _splineAnimate.Completed += OnSplineAnimateOnCompleted;
    }

    private void OnSplineAnimateOnCompleted()
    {
        _splineAnimate.Completed -= OnSplineAnimateOnCompleted;
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
