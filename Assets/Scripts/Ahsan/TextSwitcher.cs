using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TextSwitcher : MonoBehaviour
{
    public float switchInterval;
    public List<TMP_FontAsset> fontAssets;
    private int index = 0;
    public TMP_Text text;

    private void OnEnable()
    {
        StartCoroutine(CycleFont());
    }

    public IEnumerator CycleFont()
    {
        yield return new WaitForSeconds(switchInterval);
        index = (index + 1) % fontAssets.Count;
        text.font = fontAssets[index];
        if (enabled)
        {
            StartCoroutine(CycleFont());
        }
    }

    public void DisableSelf()
    {
        enabled = false;
    }
}
