using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ahsan.ScriptableObjects
{
    public enum WorldVariant
    {
        Intro,
        Anthropocene,
        PostHumanBiome,
        Signal,
        Chaos
    }

    [Serializable]
    public class Variant
    {
        public WorldVariant type;
        public SongChartPair pair;
    }

    [Serializable]
    public class Outcome
    {
        public WorldVariant type;
        public SplinePreset splinePreset;
    }


    [CreateAssetMenu(fileName = "Segment", menuName = "Scriptable Objects/Segment", order = 0)]
    public class Segment : ScriptableObject
    {
        public List<Variant> variants;
        public Dictionary<WorldVariant, SongChartPair> WorldVariants =>
            variants.ToDictionary(variant => variant.type, variant => variant.pair);

        
        
        public Outcome outcomeA;
        public KeyValuePair<WorldVariant, SplinePreset> OutcomeA => new(outcomeA.type, outcomeA.splinePreset);

        public Outcome outcomeB;
        public KeyValuePair<WorldVariant, SplinePreset> OutcomeB => new(outcomeB.type, outcomeB.splinePreset);
    }
}