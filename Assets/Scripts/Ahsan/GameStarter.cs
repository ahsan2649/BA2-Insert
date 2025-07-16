using System;
using System.Collections;
using System.Collections.Generic;
using Ahsan.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Ahsan
{
    public class GameStarter : MonoBehaviour
    {
        private static readonly int GameStart = Animator.StringToHash("gameStart");
        private static readonly int GameEnd = Animator.StringToHash("gameEnd");

        [Header("Starting UI Controls")] public Slider slider;
        public CanvasGroup textGroup;
        public float sliderFillSpeed = 0.5f;

        [Header("Event-dependent GameObjects")]
        public List<Animator> startAnimators;

        public Segment introSegment;

        public Action<Segment> OnGameStarted;
        public Action OnGameEnded;

        public NewConductor Conductor;
        private bool filled;

        void OnEnable()
        {
            slider.onValueChanged.AddListener(CallGameStart);
            OnGameStarted += StartGame;
            Conductor.OnNewSongStarted += CallGameEnd;
            OnGameEnded += EndGame
                ;
        }

        private void EndGame()
        {
            filled = false;
            foreach (var animator in startAnimators)
            {
                animator.SetTrigger(GameEnd);
            }
        }

        private void CallGameEnd(Segment segment, WorldVariant variant)
        {
            if (segment.outcomeA.type == WorldVariant.Outro && segment.outcomeB.type == WorldVariant.Outro)
            {
                StartCoroutine(ScheduleGameEnd(segment.WorldVariants[variant].audioFile.length + 2));
            }
        }

        public IEnumerator ScheduleGameEnd(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            SegmentSequencer.CurrentSegment = 0;
            OnGameEnded?.Invoke();
        }

        void OnDisable()
        {
            slider.onValueChanged.RemoveListener(CallGameStart);
            OnGameStarted -= StartGame;
            Conductor.OnNewSongStarted -= CallGameEnd;
            
        }

        void Update()
        {
            bool keysPressed = Keyboard.current.fKey.isPressed && Keyboard.current.jKey.isPressed;

            if (keysPressed)
                slider.value += Time.deltaTime * sliderFillSpeed;
            else if (!filled)
                slider.value -= Time.deltaTime;

            textGroup.alpha = 1 - slider.value;
        }

        public void CallGameStart(float value)
        {
            if (value >= 1 && !filled)
            {
                filled = true;
                OnGameStarted?.Invoke(introSegment);
            }
        }

        private void StartGame(Segment segment)
        {
            foreach (var animator in startAnimators)
            {
                animator.SetTrigger(GameStart);
            }
        }
    }
}