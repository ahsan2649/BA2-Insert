using Ahsan.ScriptableObjects;
using UnityEngine;

namespace Ahsan
{
    [CreateAssetMenu(fileName = "Song", menuName = "Scriptable Objects/Song")]
    public class SongChartPair : ScriptableObject
    {
        public AudioClip audioFile;
        public Chart chart;
        public float bpm;
        public float firstBeatOffset;
    }
}
