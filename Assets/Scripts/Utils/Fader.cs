using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour {

    public static Fader Instance;

    public Image fadeImg;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    public void FadeIn(Action callback = null)
    {
        StartCoroutine(Fade(false, callback));
    }

    public void FadeOut(Action callback = null)
    {
        StartCoroutine(Fade(true, callback));
    }
    
    IEnumerator Fade(bool ToBlack, Action callback)
    {
        float currentTime = 0;
        do
        {
            currentTime += Time.deltaTime / 2f;

            Color __alpha = fadeImg.color;
            __alpha.a = ToBlack ? currentTime : 1 - currentTime;
            fadeImg.color = __alpha;
            yield return null;

        } while (currentTime < 1f);
        
        if (callback != null) callback();
    }
}
