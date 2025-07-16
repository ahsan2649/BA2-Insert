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
        [SerializeField] private WorldVariant morpherVariant;
        [SerializeField] private bool closed;
        [SerializeField] DecisionMaker decisionMaker;
        [SerializeField] SplineContainer spline;
        [SerializeField] float morphSpeed;
#if DEBUG
        [SerializeField] SplinePreset _targetSpline;
#endif
        BezierKnot[] targetSplineKnots;
        BezierKnot[] initialSplineKnots;
        [SerializeField] private bool showDebugGUI = true;

        private void OnEnable()
        {
            decisionMaker.OnDecisionWindowEnter += segment =>
            {
                if (segment.outcomeA.type == morpherVariant)
                {
                    Debug.Log("morphing");
                    MorphTo(segment.outcomeA.splinePreset);
                }
                if (segment.outcomeB.type == morpherVariant)
                {
                    Debug.Log("morphing");
                    MorphTo(segment.outcomeB.splinePreset);
                }
            };

            decisionMaker.OnDecisionWindowExit += variant =>
            {
                MorphBack();
            };
        }

        private void Start()
        {
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

#if DEBUG
        private void OnGUI()
        {
            if (showDebugGUI)
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
        }
#endif

        void MorphTo(SplinePreset target)
        {
            targetSplineKnots = target.knots;
            if (closed)
            {
                spline.Spline.Closed = closed;
            }
        }

        void MorphBack()
        {
            targetSplineKnots = initialSplineKnots;
            if (closed)
            {
                spline.Spline.Closed = !closed;
            }
        }
    }
}