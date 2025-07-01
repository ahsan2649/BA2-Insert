using UnityEngine;

namespace Ahsan
{
    public class Conductor : MonoBehaviour
    {
        public float songBpm;
        public float secPerBeat;
        public float songPosition;
        public float songPositionInBeats;
        public float dspSongTime;
        public AudioSource musicSource;
        public float firstBeatOffset;
    
        void Start()
        {
            musicSource = GetComponent<AudioSource>();
            secPerBeat = 60f / songBpm;
            dspSongTime = (float)AudioSettings.dspTime;
            musicSource.Play();
        }

        void Update()
        {
            songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset) * 1000; //in milliseconds
            songPositionInBeats = songPosition / secPerBeat;
        }
    }
}
