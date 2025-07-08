using System;
using System.Collections;
using System.Collections.Generic;
using Ahsan.ScriptableObjects;
using UnityEngine;

namespace Ahsan
{
    public class DecisionMaker : MonoBehaviour
    {
        public List<Lane> lanes;

        public NewConductor Conductor;
        
        public int perfectHits;
        public int goodHits;
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

        private void TallyNote(float notePosition)
        {
            if (notePosition >= 1)
            {
                misses++;
            }
            else if (notePosition > 0.85)
            {
                perfectHits++;
            } else if (notePosition > 0.65)
            {
                goodHits++;
            }
        }
        
        
    }
}
