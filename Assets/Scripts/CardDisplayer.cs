using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplayer : MonoBehaviour {

    public Image cardCover;
    public Image artwork;
    public Text description;

    public Card card;

	void InitCard ()
    {
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
}
