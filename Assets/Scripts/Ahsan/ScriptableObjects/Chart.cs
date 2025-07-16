using UnityEditor;
using UnityEngine;
using System.IO;

namespace Ahsan.ScriptableObjects
{
    [System.Serializable]
    public class NoteObject
    {
        public int lane;
        public float songPosition;
        public bool isHold;
        public float holdLength;
        public bool hasSpawned = false;
    }

    [System.Serializable]
    public class ChartObject
    {
        public NoteObject[] notes;
    }

    [CreateAssetMenu(fileName = "Chart", menuName = "Scriptable Objects/Chart")]
    public class Chart : ScriptableObject
    {
        public NoteObject[] notes;

        
    }
}
