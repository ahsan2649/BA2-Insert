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
        public AudioSource musicSource1;
        public AudioSource musicSource2;
        private double nextPlaybackPosition;

        public event Action<SongChartPair> OnNewSongStarted;
        
        public NewChartParser chartParser;
        
        public SongChartPair Song1;
        public SongChartPair Song2;

        private double songStartTime;
        private float secPerBeat;
        public float songPosition;
        private float songPositionInBeats;
        private float firstBeatOffset;

        private void OnEnable()
        {
            OnNewSongStarted += ResetTimeParams;
            OnNewSongStarted += chartParser.SetCurrentChart;
        }

        private void OnDisable()
        {
            OnNewSongStarted -= ResetTimeParams;
            OnNewSongStarted -= chartParser.SetCurrentChart;
        }

        public void ResetTimeParams(SongChartPair songChartPair)
        {
            secPerBeat = 60f / songChartPair.bpm;
            firstBeatOffset = songChartPair.firstBeatOffset;
            songStartTime = (float)AudioSettings.dspTime;
        }

        // Update is called once per frame
        void Update()
        {
            songPosition = (float)(AudioSettings.dspTime - songStartTime - firstBeatOffset) * 1000; //in milliseconds
            songPositionInBeats = (songPosition / 1000f) / secPerBeat;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            { 
                PlaySongScheduled(Song1);
            }
            
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                PlaySongScheduled(Song2);
            }
            
        }

        public void PlaySongScheduled(SongChartPair songChartPair)
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
            
            targetSource.clip = songChartPair.audioFile;
            targetSource.PlayScheduled(nextPlaybackPosition);
            StartCoroutine(InvokeEventAfterDelay(nextPlaybackPosition - AudioSettings.dspTime, songChartPair));
            
            nextPlaybackPosition += songChartPair.audioFile.length;
        }
        
        private IEnumerator InvokeEventAfterDelay(double delay, SongChartPair songChartPair)
        {
            yield return new WaitForSecondsRealtime((float)delay);
            OnNewSongStarted?.Invoke(songChartPair);
        }
    }
}
