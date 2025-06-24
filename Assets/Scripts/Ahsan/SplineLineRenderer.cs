using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
[RequireComponent(typeof(LineRenderer))]
public class SplineLineRenderer : MonoBehaviour
{
    [SerializeField] SplineContainer splineContainer;
    private LineRenderer _lineRenderer;
    private Spline _spline;

    [SerializeField] private int numPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spline = splineContainer.Spline;
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = numPoints;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numPoints; i++)
        {
            var lerpValue = i / (float)(numPoints - 1);
            var lerpPoint = _spline.EvaluatePosition(lerpValue);
            _lineRenderer.SetPosition(i, transform.position + (Vector3)lerpPoint);
        }
    }
}
