using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {

    public EventTrigger[] elementsToDisable;

    public Transform confirmationDialog;

    public void Start()
    {
        ToggleButtons(false);
        Fader.Instance.FadeIn(() => ToggleButtons(true));
    }

    public void StartGame()
    {
        Fader.Instance.FadeOut(() => SceneManager.LoadScene("MainGame"));
    }

    public void ShowCredits()
    {

    }

    public void Back()
    {

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
        for (int i = 0; i < elementsToDisable.Length; ++i)
        {
            elementsToDisable[i].enabled = toggle;
        }
    }
}
