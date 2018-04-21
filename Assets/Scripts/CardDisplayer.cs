using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardDisplayer : MonoBehaviour {

    public Image cardCover;
    public Image artwork;
    public Text description;

    public Card card;

    public bool selected;

    private Outline outline;

    //DELETE, ONLY FOR DEBUG
    private void Awake()
    {
        InitCard();
    }

    public void InitCard ()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        switch (card.type)
        {
            case CardType.COMBO_INIT:
                cardCover.sprite = Resources.Load<Sprite>("BaseCards/InitCard");
                break;
            case CardType.COMBO_MIDDLE:
                cardCover.sprite = Resources.Load<Sprite>("BaseCards/MidleCard");
                break;
            case CardType.COMBO_FINISHER:
                cardCover.sprite = Resources.Load<Sprite>("BaseCards/FinisherCard");
                break;
        }

        artwork.sprite = card.artwork;
        description.text = card.description;
    }

    public void HoverCard()
    {
        if (!selected)
            transform.DOScale(new Vector3(1.35f, 1.35f, 1f), .5f);
    }

    public void ExitCard()
    {
        if (!selected)
            transform.DOScale(new Vector3(1f, 1f, 1f), .5f);
    }

    public void OnClick()
    {
        if (!selected)
        {
            transform.DOScale(new Vector3(1f, 1f, 1f), .5f);
            outline.enabled = true;
        }
        else
        {
            transform.DOScale(new Vector3(1.35f, 1.35f, 1f), .5f);
            outline.enabled = false;
        }

        selected = !selected;
    }
}
