using System;
using System.Collections.Generic;
using Ahsan.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Ahsan
{
    [Serializable]
    public class AnimatorTriggerPair
    {
        public String triggerName;
        public Animator animator;
    }
    public class GameStarter : MonoBehaviour
    {
        [Header("Starting UI Controls")]
        public Slider slider;
        public CanvasGroup textGroup;
        public float sliderFillSpeed = 0.5f;

        [Header("Event-dependent GameObjects")]
        public List<AnimatorTriggerPair> startAnimators;
        public Segment introSegment;
        
        public Action<Segment> OnGameStarted;
        
        private bool filled;
        
        void OnEnable()
        {
            slider.onValueChanged.AddListener(CallGameStart);
            OnGameStarted += StartGame;
        }

        void OnDisable()
        {
            slider.onValueChanged.RemoveListener(CallGameStart);
            OnGameStarted -= StartGame;
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
            foreach (var animatorTriggerPair in startAnimators)
            {
                animatorTriggerPair.animator.SetTrigger(animatorTriggerPair.triggerName);
            }
        }
    }
}
