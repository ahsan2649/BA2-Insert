using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    public Slider slider;
    public CanvasGroup textGroup;
    private bool _filled;
    public float sliderFillSpeed = 0.5f;

    public UnityEvent onGameStarted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.value = 0;
        slider.onValueChanged.AddListener(CallGameStart);
    }

    void CallGameStart(float value)
    {
        if (value >= 1)
        {
            slider.onValueChanged.RemoveListener(CallGameStart);
            _filled = true;
            onGameStarted.Invoke();
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.fKey.isPressed && Keyboard.current.jKey.isPressed)
        {
            slider.value += Time.deltaTime * 0.5f;
        }
        else if (!_filled)
        {
            slider.value -= Time.deltaTime;
        }
        
        textGroup.alpha = 1 - slider.value;
        
    }
}
