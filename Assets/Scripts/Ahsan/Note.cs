using System;
using Ahsan.ScriptableObjects;
using UnityEngine;
using UnityEngine.Splines;

namespace Ahsan
{
	[RequireComponent(typeof(SplineAnimate))]
	public class Note : MonoBehaviour
	{
		public SplineAnimate splineAnimate;
		public float hitTime;
        public bool isDecisionNote = false;
        public WorldVariant outcome;
        [Header("VFX")]
        [Tooltip("Assign a particle prefab that plays once and then auto‐destroys itself")]
        public GameObject hitParticlePrefab;


        private void OnDestroy()
        {
            // spawn the hit particle at this note's position
            if (hitParticlePrefab != null)
            {
                Instantiate(
                    hitParticlePrefab,
                    transform.position,
                    Quaternion.identity
                );
            }
        }
    }
}
