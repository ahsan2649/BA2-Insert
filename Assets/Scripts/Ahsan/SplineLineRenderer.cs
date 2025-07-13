using UnityEngine;
using UnityEngine.Splines;

namespace Ahsan
{
    [RequireComponent(typeof(SplineContainer))]
    [RequireComponent(typeof(LineRenderer))]
    public class SplineLineRenderer : MonoBehaviour
    {
        [SerializeField] SplineContainer splineContainer;
        private LineRenderer lineRenderer;
        private Spline spline;
        [SerializeField] private float noiseScale = 1f;
        [SerializeField] private float noiseOffset;
        [SerializeField] private float noiseSpeed = 1f;
        [SerializeField] private float displacement = 0.1f;
        [SerializeField] private Vector3 noiseDirection = new Vector3(0, 1, 0); // Y-direction by default
        [SerializeField] private int numPoints;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            spline = splineContainer.Spline;
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = numPoints;
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < numPoints; i++)
            {
                float t = i / (float)(numPoints - 1);
                Vector3 splinePoint = spline.EvaluatePosition(t);

                // 2D Perlin noise for better temporal coherence
                float noiseValue = Mathf.PerlinNoise(t * noiseScale + noiseOffset, Time.time * noiseSpeed);
                float offset = (noiseValue - 0.5f) * 2f * displacement;

                if (i == 0 || i == numPoints - 1)
                {
                    offset = 0;
                }
                
                Vector3 displaced = splinePoint + noiseDirection.normalized * offset;
                lineRenderer.SetPosition(i, transform.TransformPoint(displaced));
            }
        }
    }
}
