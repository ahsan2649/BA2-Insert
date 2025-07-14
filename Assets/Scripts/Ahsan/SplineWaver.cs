using System;
using Ahsan;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class SplineWaver : MonoBehaviour
{
    Spline _spline;
    private Vector3[] _initialKnotPositions;

    [SerializeField] DecisionMaker decisionMaker;
    private bool isMakingDecision;
    [SerializeField] private Vector3 waveDirection;
    [SerializeField] private float frequency = 1;
    [SerializeField] private float amplitude = 1;
    [SerializeField] private float speed = 1;
    [SerializeField] private float offset = 1;
    [SerializeField] private bool reverse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        if (decisionMaker)
        {
            decisionMaker.OnDecisionWindowEnter += segment =>
            {
                enabled = false;
            };

            decisionMaker.OnDecisionWindowExit += variant =>
            {
                enabled = true;
            };
        }
    }

    void Start()
    {
        _spline = GetComponent<SplineContainer>().Spline;
        _initialKnotPositions = new Vector3[_spline.Count];
        for (int i = 0; i < _spline.Count; i++)
        {
            _initialKnotPositions[i] = _spline[i].Position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMakingDecision)
        {
            for(int i = 1; i < _spline.Count - 1; i++)
            {
                float t = (float)i / (_spline.Count - 1);
                if (reverse)
                {
                    t = 1.0f - t;
                }
                float waveOffset = Mathf.Sin(t * frequency * Mathf.PI * 2f + Time.time * speed + offset) * amplitude;
                Vector3 displacedPosition = _initialKnotPositions[i] + waveDirection.normalized * waveOffset;
                _spline[i] = new BezierKnot(displacedPosition, _spline[i].TangentIn, _spline[i].TangentOut, _spline[i].Rotation);
            }
        }
    }
}
