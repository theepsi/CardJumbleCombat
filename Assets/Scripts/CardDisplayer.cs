using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardDisplayer : MonoBehaviour {

    public Image cardCover;
    public Image artwork;
    public Text description;
    public Card card;

    public bool selected;

    public void InitCard (Card card)
    {
        transform.DORestart();
        this.card = card;
        selected = false;

        switch (card.type)
        {
            case CardType.COMBO_INIT:
                cardCover.sprite = Resources.Load<Sprite>("BaseCards/InitCard");
                break;
            case CardType.COMBO_MIDDLE:
                cardCover.sprite = Resources.Load<Sprite>("BaseCards/MiddleCard");
                break;
            case CardType.COMBO_FINISHER:
                cardCover.sprite = Resources.Load<Sprite>("BaseCards/FinisherCard");
                break;
        }

        artwork.sprite = card.artwork;
        description.text = card.description;

        transform.DOScale(new Vector3(1f, 1f, 1f), .5f);
        transform.DOLocalMoveY(0, .5f);

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
            transform.DOLocalMoveY(25f, .5f);
        }
        else
        {
            transform.DOScale(new Vector3(1.35f, 1.35f, 1f), .5f);
            transform.DOLocalMoveY(0, .5f);
        }

        selected = !selected;
    }
}
