using System;
using System.Collections.Generic;
using System.Linq;
using Ahsan.ScriptableObjects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Ahsan
{
    public class SplineMorpher : MonoBehaviour
    {
        [SerializeField] SplineContainer spline;
        [SerializeField] float morphSpeed;
#if DEBUG
        [SerializeField] SplinePreset _targetSpline;
#endif

        BezierKnot[] targetSplineKnots;
        BezierKnot[] initialSplineKnots;


        private void Start()
        {
            spline = GetComponent<SplineContainer>();
            initialSplineKnots = spline.Spline.ToArray();
            targetSplineKnots = spline.Spline.ToArray();
        }

        private void Update()
        {
            for (int i = 0; i < spline.Spline.Count; i++)
            {
                spline.Spline[i] = BezierKnotLerp(spline.Spline[i], targetSplineKnots[i],
                    1 - Mathf.Exp(-morphSpeed * Time.deltaTime));
            }
        }

        private BezierKnot BezierKnotLerp(BezierKnot a, BezierKnot b, float t)
        {
            return new BezierKnot(
                math.lerp(a.Position, b.Position, t),
                math.lerp(a.TangentIn, b.TangentIn, t),
                math.lerp(a.TangentOut, b.TangentOut, t),
                Quaternion.Lerp(a.Rotation, b.Rotation, t)
            );
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 100, 100), "Morph"))
            {
                MorphTo(_targetSpline);
            }

            if (GUI.Button(new Rect(10, 220, 200, 100), "Morph Back"))
            {
                MorphBack();
            }
        }

        void MorphTo(SplinePreset target)
        {
            targetSplineKnots = target.knots;
        }

        void MorphBack()
        {
            targetSplineKnots = initialSplineKnots;
        }
    }
}