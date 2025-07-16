using System;
using System.Collections;
using System.Collections.Generic;
using Ahsan.ScriptableObjects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Ahsan
{
    public class DecisionMaker : MonoBehaviour
    {
        public List<Lane> lanes;
        public NewConductor conductor;

        private bool isDecisionMade = false;

        public int hitCombo;
        public int missCombo;

        public int perfectHits;
        public int goodHits;
        public int greatHits;
        public int misses;

        public event Action<Segment> OnDecisionWindowEnter;
        public event Action<WorldVariant> OnDecisionWindowExit;

        public WorldVariant selectedVariant = WorldVariant.Anthropocene;

        private IEnumerator InvokeDecisionWindowEnter(double delay, Segment segment)
        {
            yield return new WaitForSecondsRealtime((float)delay);
            OnDecisionWindowEnter?.Invoke(segment);
        }

        private IEnumerator InvokeDecisionWindowExit(double delay)
        {
            yield return new WaitForSecondsRealtime((float)delay);
            OnDecisionWindowExit?.Invoke(selectedVariant);
        }

        private void OnEnable()
        {
            foreach (Lane lane in lanes)
            {
                lane.OnNoteDestroyed += TallyNote;
            }

            conductor.OnNewSongStarted += ResetAndScheduleDecisionWindow;
            OnDecisionWindowEnter += SpawnDecisionNotes;

            lanes[0].OnDecisionNote += MakeDecision;
            lanes[1].OnDecisionNote += MakeDecision;
        }

        private void SpawnDecisionNotes(Segment segment)
        {
            if (segment.outcomeA.type == WorldVariant.Outro || segment.outcomeB.type == WorldVariant.Outro)
            {
                return;
            }
            lanes[0].SpawnDecisionNote(segment.outcomeA.decisionNotePrefab, 50 / 10, conductor.songPosition + 50 * 100);
            lanes[1].SpawnDecisionNote(segment.outcomeB.decisionNotePrefab, 50 / 10, conductor.songPosition + 50 * 100);
        }

        private void MakeDecision(WorldVariant variant)
        {
            if (isDecisionMade)
            {
                return;
            }

            WorldVariant val;
            if (hitCombo > 5)
            {
                val = WorldVariant.Anthropocene;
            }
            else if (missCombo > 5)
            {
                val = WorldVariant.PostHumanBiome;
            }
            else if (perfectHits > 3 || greatHits > 5)
            {
                val = WorldVariant.Signal;
            }
            else
            {
                val = (WorldVariant)Random.Range(1, 4);
            }
            
            selectedVariant = DecisionMatrix(val, variant);
            isDecisionMade = true;
        }

        private WorldVariant DecisionMatrix(WorldVariant playStyle, WorldVariant decision)
        {
            switch (SegmentSequencer.CurrentSegment)
            {
                case 0:
                    switch (decision)
                    {
                        case WorldVariant.Anthropocene:
                            switch (playStyle)
                            {
                                case WorldVariant.Anthropocene:
                                case WorldVariant.PostHumanBiome:
                                case WorldVariant.Signal:
                                    return WorldVariant.Anthropocene;
                                case WorldVariant.Chaos:
                                    return WorldVariant.Chaos;
                            }

                            break;
                        case WorldVariant.PostHumanBiome:
                            switch (playStyle)
                            {
                                case WorldVariant.Anthropocene:
                                case WorldVariant.PostHumanBiome:
                                case WorldVariant.Chaos:
                                    return WorldVariant.PostHumanBiome;
                                case WorldVariant.Signal:
                                    return WorldVariant.Chaos;
                            }

                            break;
                    }

                    break;
                case 1:
                    switch (decision)
                    {
                        case WorldVariant.Signal:
                            switch (playStyle)
                            {
                                case WorldVariant.Anthropocene:
                                case WorldVariant.Signal:
                                case WorldVariant.Chaos:
                                    return WorldVariant.Signal;
                                case WorldVariant.PostHumanBiome:
                                    return WorldVariant.PostHumanBiome;
                            }

                            break;
                        case WorldVariant.Chaos:
                            switch (playStyle)
                            {
                                case WorldVariant.Anthropocene:
                                    return WorldVariant.Anthropocene;
                                case WorldVariant.PostHumanBiome:
                                case WorldVariant.Signal:
                                case WorldVariant.Chaos:
                                    return WorldVariant.Chaos;
                            }

                            break;
                    }

                    break;
                case 2:
                    switch (decision)
                    {
                        case WorldVariant.Anthropocene:
                            return playStyle;
                        case WorldVariant.Signal:
                            switch (playStyle)
                            {
                                case WorldVariant.Anthropocene:
                                case WorldVariant.Signal:
                                    return WorldVariant.Anthropocene;
                                case WorldVariant.PostHumanBiome:
                                    return WorldVariant.Signal;
                                case WorldVariant.Chaos:
                                    return WorldVariant.Chaos;
                            }

                            break;
                    }

                    break;
                case 3:
                    switch (decision)
                    {
                        case WorldVariant.PostHumanBiome:
                            switch (playStyle)
                            {
                                case WorldVariant.Anthropocene:
                                    return WorldVariant.Anthropocene;
                                case WorldVariant.PostHumanBiome:
                                    return WorldVariant.PostHumanBiome;
                                case WorldVariant.Signal:
                                    return WorldVariant.Signal;
                                case WorldVariant.Chaos:
                                    return WorldVariant.PostHumanBiome;
                            }

                            break;
                        case WorldVariant.Chaos:
                            switch (playStyle)
                            {
                                case WorldVariant.Anthropocene:
                                    return WorldVariant.Anthropocene;
                                case WorldVariant.PostHumanBiome:
                                    return WorldVariant.PostHumanBiome;
                                case WorldVariant.Signal:
                                case WorldVariant.Chaos:
                                    return WorldVariant.Chaos;
                            }

                            break;
                    }

                    break;
                case 4:
                    switch (decision)
                    {
                        case WorldVariant.Anthropocene:
                            return playStyle;
                        case WorldVariant.Chaos:
                            switch (playStyle)
                            {
                                case WorldVariant.Anthropocene:
                                    return WorldVariant.Anthropocene;
                                    break;
                                case WorldVariant.PostHumanBiome:
                                    return WorldVariant.PostHumanBiome;
                                    break;
                                case WorldVariant.Signal:
                                    return WorldVariant.Chaos;
                                    break;
                                case WorldVariant.Chaos:
                                    return WorldVariant.Chaos;
                                    break;
                            }

                            break;
                    }

                    break;
            }
            
            return WorldVariant.Chaos;
        }

        private void OnDisable()
        {
            foreach (Lane lane in lanes)
            {
                lane.OnNoteDestroyed -= TallyNote;
            }

            conductor.OnNewSongStarted -= ResetAndScheduleDecisionWindow;
        }

        private void ResetAndScheduleDecisionWindow(Segment segment, WorldVariant type)
        {
            perfectHits = 0;
            goodHits = 0;
            greatHits = 0;
            misses = 0;
            isDecisionMade = false;

            if (segment.outcomeA.type == WorldVariant.Outro || segment.outcomeB.type == WorldVariant.Outro)
            {
                return;
            }
            
            StartCoroutine(InvokeDecisionWindowEnter(segment.WorldVariants[type].decisionWindowStart / 1000, segment));
            StartCoroutine(InvokeDecisionWindowExit(segment.WorldVariants[type].decisionWindowEnd / 1000));
        }


        private void TallyNote(float noteHitTime)
        {
            // a perfect hit is when noteposition is 0.9~ as any after 1 is too late and it gets auto deleted, having late hits is impossible with this setup
            // scroll speed will change the timings of the notes
            // scroll speed != difficulty
            // higher scroll speeds should not differ the scoring of the song
            // comparing against the note's chart spawn time vs the conductor current time will fix both these issues

            // https://iidx.org/assets/img/timing_diagram.png

            // When timing windows overlap with notes close to eachother it gets a bit fucky but i just wont design charts like that :)

            float hitDifference = math.floor(noteHitTime - conductor.songPosition) - 255;
            // 500ms range
            print(hitDifference);
            if (hitDifference <= -250)
            {
                // miss
                missCombo++;
                misses++;
                hitCombo = 0;
            }
            else if (hitDifference <= -150)
            {
                // good
                goodHits++;
                hitCombo++;
                missCombo = 0;
            }
            else if (hitDifference <= -75)
            {
                // great
                greatHits++;
                hitCombo++;
                missCombo = 0;
            }
            else if (hitDifference <= 75)
            {
                // perfect
                perfectHits++;
                hitCombo++;
                missCombo = 0;
            }
            else if (hitDifference <= 150)
            {
                // great
                greatHits++;
                hitCombo++;
                missCombo = 0;
            }
            else if (hitDifference <= 250)
            {
                // good
                goodHits++;
                hitCombo++;
                missCombo = 0;
            }
            else
            {
                // miss
                missCombo++;
                misses++;
                hitCombo = 0;
            }
        }
    }
}