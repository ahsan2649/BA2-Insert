using System;
using System.Collections;
using Ahsan.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Ahsan
{
    public class NewConductor : MonoBehaviour
    {
        [SerializeField] private DecisionMaker decisionMaker;
        [SerializeField] private GameStarter gameStarter;
        [SerializeField] private SegmentSequencer segmentSequencer;
        [SerializeField] private AudioSource musicSource1;
        [SerializeField] private AudioSource musicSource2;

        [HideInInspector] public float songPosition;
        [HideInInspector] public float songPositionInBeats;

        [SerializeField] private bool showDebugGUI = false;
        
        private double nextPlaybackPosition;
        private double songStartTime;
        private float secPerBeat;
        private float firstBeatOffset;
        public event Action<Segment, WorldVariant> OnNewSongStarted;

        private void OnEnable()
        {
            gameStarter.OnGameStarted += PlayIntroSegment;
            OnNewSongStarted += ResetTimeParams;
            decisionMaker.OnDecisionWindowExit += variant =>
            {
                PlaySegmentScheduled(segmentSequencer.segments[SegmentSequencer.CurrentSegment], variant);
            };
        }

        private void PlayIntroSegment(Segment segment)
        {
            StartCoroutine(DelayedStart(segment, 2));
        }

        private IEnumerator DelayedStart(Segment segment, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            Debug.Log(segment);
            PlaySegmentScheduled(segment, WorldVariant.Intro);
        }

        private void OnDisable()
        {
            gameStarter.OnGameStarted -= PlayIntroSegment;
            OnNewSongStarted -= ResetTimeParams;
        }

        public void ResetTimeParams(Segment segment, WorldVariant type)
        {
            secPerBeat = 60f / segment.WorldVariants[type].bpm;
            firstBeatOffset = segment.WorldVariants[type].firstBeatOffset;
            songStartTime = (float)AudioSettings.dspTime;
        }

		// Update is called once per frame
		void Update()
        {
            songPosition = (float)(AudioSettings.dspTime - songStartTime - firstBeatOffset) * 1000; //in milliseconds
            songPositionInBeats = (songPosition / 1000f) / secPerBeat;
        }

        public void PlaySegmentScheduled(Segment segment, WorldVariant type)
        {
            AudioSource targetSource = !musicSource1.isPlaying ? musicSource1 : !musicSource2.isPlaying ? musicSource2 : null;

            if (!targetSource)
            {
                return;
            }

            if (!musicSource1.isPlaying && !musicSource2.isPlaying)
            {
                nextPlaybackPosition = AudioSettings.dspTime;
            }

            var selectedChart = segment.WorldVariants[type];
            
            targetSource.clip = selectedChart.audioFile;
            targetSource.PlayScheduled(nextPlaybackPosition);
            StartCoroutine(InvokeEventAfterDelay(nextPlaybackPosition - AudioSettings.dspTime, segment, type));
            
            nextPlaybackPosition += selectedChart.audioFile.length;
        }
        
        private IEnumerator InvokeEventAfterDelay(double delay, Segment segment, WorldVariant type)
        {
            yield return new WaitForSecondsRealtime((float)delay);
            OnNewSongStarted?.Invoke(segment, type);
        }

        private void OnGUI()
        {
            if (showDebugGUI)
            {
            GUI.Box(new Rect(0, 0, 500, 250), songPosition.ToString());
                
            }
        }
    }
}
