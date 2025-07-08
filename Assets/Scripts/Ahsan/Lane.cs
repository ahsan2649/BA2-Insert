using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

namespace Ahsan
{
	public class Lane : MonoBehaviour
	{
		[SerializeField] SplineContainer spline;
		[SerializeField] Note notePrefab;
		[SerializeField] InputAction key;

		List<Note> notes = new();

		public event Action<float> OnNoteDestroyed;
	
		private void OnEnable()
		{
			key.Enable();
			key.performed += Hit;
		
		}

		private void OnDisable()
		{
			key.performed -= Hit;
			key.Disable();
		}

		void Hit(InputAction.CallbackContext context)
		{
			if (notes.Count == 0)
			{
				return;
			}
			var note = notes[0];
			if (note.splineAnimate.NormalizedTime < 0.5) // if leading note is less than halfway, don't register (adjust to designer preference)
			{
				return;
			}
			notes.Remove(note);
			DestroyNote(note);
		}

		// Call this function to spawn a note.
		// Duration means how long the note takes to go from one end to the other
		public void SpawnNote(float duration)
		{
			var note = Instantiate(notePrefab, spline.transform.position, spline.transform.rotation);
			notes.Add(note);

			note.splineAnimate.Container = spline;
			note.splineAnimate.Duration = duration;

			note.splineAnimate.Completed += (() =>
			{
				notes.Remove(note);
				DestroyNote(note);
			});
		
			note.splineAnimate.Play();
		}

		public void DestroyNote(Note note)
		{
			OnNoteDestroyed?.Invoke(note.splineAnimate.NormalizedTime);
			Destroy(note.gameObject);
		}
	

		public float GetSplineLength()
		{
			// https://docs.unity3d.com/Packages/com.unity.splines@2.5/api/UnityEngine.Splines.SplineSlice-1.GetLength.html
			// this isnt real? if you could figure this out that would be awesome
			// return spline.GetLength()
			return 30;
		}
	}
}