﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public EventTrigger[] elementsToDisable;

    public Transform confirmationDialog;

    public Slider difficultSelection;

    public void Start()
    {
        ToggleButtons(false);
        difficultSelection.value = EnemyDifficulty.AIDifficulty;
        Fader.Instance.FadeIn(() => ToggleButtons(true));
        BackgroundMusic.Instance.FadeIn();
    }

    public void StartGame()
    {
        ToggleButtons(false);
        EnemyDifficulty.AIDifficulty = (int) difficultSelection.value;
        BackgroundMusic.Instance.FadeOut();
        Fader.Instance.FadeOut(() => SceneManager.LoadScene("MainGame"));
    }

    public void ShowConfirmation()
    {
        ToggleButtons(false);
        confirmationDialog.gameObject.SetActive(true);
        confirmationDialog.DOLocalMoveY(0, .5f);//.DOJump(new Vector3(0f, 0f, 0f), 3f, 1, 1f);
    }

    public void DismissConfirmation()
    {
        confirmationDialog.DOLocalMoveY(-900f, .5f).OnComplete(() => confirmationDialog.gameObject.SetActive(false)); ;//confirmationDialog.GetComponent<RectTransform>().DOLocalJump(new Vector3(0f, -900f, 0f), 3f, 1, 1f).OnComplete(() => confirmationDialog.gameObject.SetActive(false));
        ToggleButtons(true);
    }

    public void Quit(Transform data)
    {
        ShowConfirmation();

        data.DOScale(new Vector3(1f, 1f, 1f), .5f);
    }

    public void ButtonEnter(Transform data)
    {
        data.DOScale(new Vector3(1.3f, 1.3f, 1f), .5f);
    }

    public void ButtonExit(Transform data)
    {
        data.DOScale(new Vector3(1f, 1f, 1f), .5f);
    }

    public void ConfirmationYes()
    {
        Fader.Instance.FadeOut(() => Application.Quit());
    }

    public void ConfirmationNo()
    {
        DismissConfirmation();
    }

    public void ToggleButtons(bool toggle)
    {
        difficultSelection.interactable = toggle;
        for (int i = 0; i < elementsToDisable.Length; ++i)
        {
            elementsToDisable[i].enabled = toggle;
        }
    }
}
