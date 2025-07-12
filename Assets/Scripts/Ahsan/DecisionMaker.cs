using System;
using System.Collections;
using System.Collections.Generic;
using Ahsan.ScriptableObjects;
using Unity.Mathematics;
using UnityEngine;

namespace Ahsan
{
    public class DecisionMaker : MonoBehaviour
    {
        public List<Lane> lanes;

        public NewConductor Conductor;

		public int hitCombo;
		public int missCombo;

		public int perfectHits;
        public int goodHits;
		public int greatHits;
		public int misses;
    
        public event Action OnDecisionWindowEnter;
        public event Action OnDecisionWindowExit;

        
        private IEnumerator InvokeDecisionWindowEnter(double delay)
        {
            yield return new WaitForSecondsRealtime((float)delay);
            OnDecisionWindowEnter?.Invoke();
        }
        
        private IEnumerator InvokeDecisionWindowExit(double delay)
        {
            yield return new WaitForSecondsRealtime((float)delay);
            OnDecisionWindowExit?.Invoke();
        }
        
        private void OnEnable()
        {
            foreach (Lane lane in lanes)
            {
                lane.OnNoteDestroyed += TallyNote;
            }

            Conductor.OnNewSongStarted += ScheduleDecisionWindow;
        }

        private void ScheduleDecisionWindow(SongChartPair pair)
        {
            StartCoroutine(InvokeDecisionWindowEnter(pair.decisionWindowStart));
            StartCoroutine(InvokeDecisionWindowExit(pair.decisionWindowEnd));
        }

        private void OnDisable()
        {
            foreach (Lane lane in lanes)
            {
                lane.OnNoteDestroyed -= TallyNote;
            }
            Conductor.OnNewSongStarted -= ScheduleDecisionWindow;
        }

		private void TallyNote(float noteHitTime)
		{
			// a perfect hit is when noteposition is 0.9~ as any after 1 is too late and it gets auto deleted, having late hits is impossible with this setup
			// scroll speed will change the timings of the notes
			// scroll speed != difficulty
			// higher scroll speeds should not differ the scoring of the song
			// comparing against the note's chart spawn time vs the conductor current time will fix both these issues

			// https://iidx.org/assets/img/timing_diagram.png

			// When timing windows overlap with notes close to eachother it gets a bit fucky but i just wont design charts like that :)

			float hitDifference = math.floor(noteHitTime - Conductor.songPosition) - 255;
			// 500ms range
			print(hitDifference);
			if (hitDifference <= -250)
			{
				// miss
				missCombo++;
				misses++;
				hitCombo = 0;
			}
			else if (hitDifference <= -150)
			{
				// good
				goodHits++;
				hitCombo++;
				missCombo = 0;
			}
			else if (hitDifference <= -75)
			{
				// great
				greatHits++;
				hitCombo++;
				missCombo = 0;
			}
			else if (hitDifference <= 75)
			{
				// perfect
				perfectHits++;
				hitCombo++;
				missCombo = 0;
			}
			else if (hitDifference <= 150)
			{
				// great
				greatHits++;
				hitCombo++;
				missCombo = 0;
			}
			else if (hitDifference <= 250)
			{
				// good
				goodHits++;
				hitCombo++;
				missCombo = 0;
			}
			else
			{
				// miss
				missCombo++;
				misses++;
				hitCombo = 0;
			}
		}


	}
}
