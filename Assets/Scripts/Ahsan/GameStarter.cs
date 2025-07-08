using System;
using Ahsan;
using Ahsan.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    private static readonly int Fade = Animator.StringToHash("fade");
    public Slider slider;
    public CanvasGroup textGroup;
    private bool filled;
    public float sliderFillSpeed = 0.5f;

    [Header("Event-dependent GameObjects")]
    public Animator sliderAnimator;

    public NewConductor conductor;
    public SongChartPair startingSong;
    public Action OnGameStarted;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        slider.onValueChanged.AddListener(CallGameStart);
        OnGameStarted += StartGame;
    }

    private void StartGame()
    {
        OnGameStarted -= StartGame;
        sliderAnimator.SetTrigger(Fade);
        conductor.PlaySongScheduled(startingSong);
    }

    public void CallGameStart(float value)
    {
        if (value >= 1 && !filled)
        {
            filled = true;
            OnGameStarted?.Invoke();
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        bool keysPressed = Keyboard.current.fKey.isPressed && Keyboard.current.jKey.isPressed;

        if (keysPressed)
            slider.value += Time.deltaTime * sliderFillSpeed;
        else if (!filled)
            slider.value -= Time.deltaTime;

        textGroup.alpha = 1 - slider.value;
    }
}
