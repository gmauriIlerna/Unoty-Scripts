using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> cards = new List<Card>();
    private Stack<Card> drawPile = new Stack<Card>();

    public Deck(Dictionary<string, Sprite> cardSprites)
    {
        InitializeDeck(cardSprites);
        ShuffleDeck();
    }

    private void InitializeDeck(Dictionary<string, Sprite> cardSprites)
    {
        Card.CardColor[] colors = { Card.CardColor.Red, Card.CardColor.Blue, Card.CardColor.Green, Card.CardColor.Yellow };

        // Numbered cards (0-9, two of each except 0)
        foreach (var color in colors)
        {
            for (int num = 0; num <= 9; num++)
            {
                string spriteKey = $"{color}_{num}"+"_0";
                Sprite sprite = cardSprites.ContainsKey(spriteKey) ? cardSprites[spriteKey] : null;
                cards.Add(new Card(color, Card.CardType.Number, num, sprite));

                if (num != 0)
                    cards.Add(new Card(color, Card.CardType.Number, num, sprite));
            }

            // Special cards (2 of each per color)
            Card.CardType[] specialTypes = { Card.CardType.Skip, Card.CardType.Reverse, Card.CardType.Draw };
            foreach (var type in specialTypes)
            {
                string spriteKey = $"{color}_{type}"+"_0";
                Sprite sprite = cardSprites.ContainsKey(spriteKey) ? cardSprites[spriteKey] : null;
                cards.Add(new Card(color, type, -1, sprite));
                cards.Add(new Card(color, type, -1, sprite));
            }
        }

        // Wild and Wild Draw Four cards (4 of each)
        for (int i = 0; i < 4; i++)
        {
            cards.Add(new Card(Card.CardColor.Wild, Card.CardType.Wild, -1, cardSprites["Wild_0"]));
            cards.Add(new Card(Card.CardColor.Wild, Card.CardType.Wild_Draw, -1, cardSprites["Wild_Draw_0"]));
        }

        // Move cards to draw pile
        foreach (var card in cards)
        {
            drawPile.Push(card);
        }
    }

    private void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        List<Card> shuffled = new List<Card>(drawPile);
        int n = shuffled.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (shuffled[n], shuffled[k]) = (shuffled[k], shuffled[n]);
        }
        drawPile = new Stack<Card>(shuffled);
    }

    public Card DrawCard()
    {
        return drawPile.Count > 0 ? drawPile.Pop() : null;
    }

    public int RemainingCards()
    {
        return drawPile.Count;
    }
}
