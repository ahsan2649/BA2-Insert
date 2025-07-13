using System;
using System.Collections.Generic;
using Ahsan;
using Ahsan.ScriptableObjects;
using UnityEngine;

public class SegmentSequencer : MonoBehaviour
{
    public DecisionMaker decisionMaker;
    public NewConductor newConductor;
    public List<Segment> segments;
    public int currentSegment;
    public WorldVariant currentWorldVariant = WorldVariant.Anthropocene;

    private void OnEnable()
    {
        decisionMaker.OnDecisionWindowExit += SetCurrentWorldVariant;
        newConductor.OnNewSongStarted += QueueNextSegment;
    }

    private void QueueNextSegment(Segment segment, WorldVariant variant)
    {
        currentSegment++;
    }

    private void SetCurrentWorldVariant(WorldVariant variant)
    {
        currentWorldVariant = variant;
    }
}
