using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour {

    public static BackgroundMusic Instance;

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
        
        audioSource = GetComponent<AudioSource>();
    }

    public void FadeIn(Action callback = null)
    {
        audioSource.volume = 1;
        audioSource.Play();
        //StartCoroutine(FadeMusic(true, callback));
    }

    public void FadeOut(Action callback = null)
    {
        StartCoroutine(FadeMusic(false, () =>
        {
            if (callback != null)
                callback();
            audioSource.Stop();
        }));
    }

    IEnumerator FadeMusic(bool fadein, Action callback)
    {
        float currentTime = 0;
        do
        {
            currentTime += Time.deltaTime / 2f;

            audioSource.volume = fadein ? currentTime : 1 - currentTime;

            yield return null;

        } while (currentTime < 1f);

        if (callback != null) callback();
    }
}
