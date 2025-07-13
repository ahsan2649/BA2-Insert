using UnityEngine;

namespace Ahsan.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Song-Chart Pair", menuName = "Scriptable Objects/Song-Chart Pair")]
    public class SongChartPair : ScriptableObject
    {
        public AudioClip audioFile;
        public Chart chart;
        public float bpm;
        public float firstBeatOffset;
        public float decisionWindowStart;
        public float decisionWindowEnd;
    }
}
