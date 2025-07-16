using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Ahsan.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SplinePreset", menuName = "Scriptable Objects/Spline Preset")]
    public class SplinePreset : ScriptableObject
    {
        public BezierKnot[] knots;

        
    }
}
