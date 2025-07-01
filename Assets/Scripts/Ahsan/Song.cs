using UnityEngine;

namespace Ahsan
{
    [CreateAssetMenu(fileName = "Song", menuName = "Scriptable Objects/Song")]
    public class Song : ScriptableObject
    {
        public AudioClip audioFile;
        public float bpm;
        public float firstBeatOffset;
    }
}
