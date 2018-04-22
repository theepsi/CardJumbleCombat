using UnityEngine;

public enum CardType
{
    COMBO_INIT,
    COMBO_MIDDLE,
    COMBO_FINISHER
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {

    public Sprite artwork;
    public string description;

    public int damage;

    public CardType type;
    
    public void Print()
    {
        Debug.Log(description + "-" + type);
    }
	
}
