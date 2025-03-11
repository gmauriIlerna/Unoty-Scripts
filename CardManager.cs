using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    public GameObject cardPrefab; // The prefab for displaying cards in the scene
    public Transform[] playerHands; // Array of empty GameObjects representing player hands

    private Deck deck;
    private Dictionary<Transform, List<Card>> playerCards = new Dictionary<Transform, List<Card>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeDeck();
        DealStartingCards();
    }

    private void InitializeDeck()
    {
        Dictionary<string, Sprite> cardSprites = LoadCardSprites();
        deck = new Deck(cardSprites);
    }

    private void DealStartingCards()
    {
        foreach (Transform hand in playerHands)
        {
            playerCards[hand] = new List<Card>();
            for (int i = 0; i < 7; i++)
            {
                DrawCard(hand);
            }
        }
    }

    public void DrawCard(Transform playerHand)
    {
        Card drawnCard = deck.DrawCard();
        if (drawnCard != null)
        {
            playerCards[playerHand].Add(drawnCard);
            InstantiateCardObject(drawnCard, playerHand);
        }
    }

    private void InstantiateCardObject(Card card, Transform parent)
    {
        // Instantiate the card prefab
        GameObject cardObject = Instantiate(cardPrefab, parent);
        cardObject.GetComponent<SpriteRenderer>().sprite = card.CardSprite;

        // Add CardData to the card object
        CardData cardData = cardObject.AddComponent<CardData>();
        cardData.card = card; // Assign the Card object to the CardData component

        // Add card to the player's hand list
        PlayerHand playerHand = parent.GetComponent<PlayerHand>();
        if (playerHand != null)
        {
            playerHand.AddCardToHand(cardObject);
        }

        // Position card in hand
        int cardIndex = playerHand.cardsInHand.Count - 1;
        float cardSpacing = 300f;
        float startX = -((playerHand.cardsInHand.Count - 1) * cardSpacing) / 2;
        cardObject.transform.localPosition = new Vector3(startX + cardIndex * cardSpacing, 0, 0);
    }

    private Dictionary<string, Sprite> LoadCardSprites()
    {
        Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Uno Game Assets");

        foreach (Sprite sprite in loadedSprites)
        {
            sprites[sprite.name] = sprite;
        }

        return sprites;
    }
}
