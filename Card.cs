using UnityEngine;

public class Card
{
    public enum CardColor { Red, Blue, Green, Yellow, Wild }
    public enum CardType { Number, Skip, Reverse, Draw, Wild, Wild_Draw}

    public CardColor Color { get; private set; }
    public CardType Type { get; private set; }
    public int Number { get; private set; } // Only relevant for number cards
    public Sprite CardSprite { get; private set; } // Holds the visual representation of the card

    public Card(CardColor color, CardType type, int number, Sprite sprite)
    {
        Color = color;
        Type = type;
        Number = number;
        CardSprite = sprite;
    }

    public override string ToString()
    {
        return Type == CardType.Number ? $"{Color} {Number}" : $"{Color} {Type}";
    }
}
