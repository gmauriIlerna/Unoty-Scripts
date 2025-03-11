using System.Collections.Generic;
using UnityEngine;
using static Card;

public class PlayerHand : MonoBehaviour
{
    public List<GameObject> cardsInHand = new List<GameObject>();
    public Transform discardPile; // Assign this in the inspector
    private int selectedIndex = 0;
    private float globalScale = 30f;

    public bool isPlayerOne = true; // Set this in the Inspector (true = P1, false = P2)

    private float[] cardPositions = new float[7]; // Positions for the cards

    void Start()
    {
        // Initialize the positions for the cards (X values)
        for (int i = 0; i < cardPositions.Length; i++)
        {
            cardPositions[i] = i * 150f; // X = 0, 150, 300, ...
        }
    }

    void Update()
    {
        if (!isPlayerOne) return; // Ignore input if it's not P1's hand
        if (cardsInHand.Count == 0) return;

        // Navigate through cards
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectCard((selectedIndex - 1 + cardsInHand.Count) % cardsInHand.Count);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectCard((selectedIndex + 1) % cardsInHand.Count);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameManager.Instance.IsPlayerOneTurn)
            {
                PlayCard();
            } else
            {
                Debug.Log("Not player's turn");
            }
        }

        // Draw a new card when 'E' is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            DrawNewCard();
        }
    }

    private void SelectCard(int newIndex)
    {
        if (cardsInHand.Count == 0) return;

        // Reset previous selection
        cardsInHand[selectedIndex].transform.localScale = Vector3.one * globalScale;

        // Update selection
        selectedIndex = newIndex;
        cardsInHand[selectedIndex].transform.localScale = Vector3.one * 1.2f * globalScale; // Slightly enlarge
    }

    private void PlayCard()
    {
        if (cardsInHand.Count == 0) return;

        GameObject selectedCard = cardsInHand[selectedIndex];
        CardData selectedCardData = selectedCard.GetComponent<CardData>(); // Get card data

        if (selectedCardData == null)
        {
            Debug.LogError("Missing CardData on card GameObject!");
            return;
        }

        // Get the last played card (excluding wild cards)
        Card lastCardPlayed = null;
        if (discardPile.childCount > 0)
        {
            lastCardPlayed = discardPile.GetChild(0).GetComponent<CardData>().card;
        }

        // Check if the selected card follows Uno rules
        bool isValidPlay = false;

        if (lastCardPlayed == null)
        {
            // First card can be anything
            isValidPlay = true;
        }
        else
        {
            // Wild cards and Wild Draw cards can always be played, and after one, any card can be played
            if (selectedCardData.card.Type == CardType.Wild || selectedCardData.card.Type == CardType.Wild_Draw)
            {
                isValidPlay = true;
            }
            else
            {
                // If the last card was a wild card or wild draw, allow any card to be played next
                if (lastCardPlayed.Type == CardType.Wild || lastCardPlayed.Type == CardType.Wild_Draw)
                {
                    isValidPlay = true;
                }
                else
                {
                    // For number cards, check if color or number matches
                    if (selectedCardData.card.Type == CardType.Number)
                    {
                        isValidPlay = (selectedCardData.card.Color == lastCardPlayed.Color) || // Same color
                                       (selectedCardData.card.Number == lastCardPlayed.Number); // Same number
                    }
                    // For action cards (Skip, Reverse, etc.), check if the same type of action is played
                    else if (selectedCardData.card.Type == lastCardPlayed.Type)
                    {
                        isValidPlay = true; // Same color
                    }
                    else if (selectedCardData.card.Color == lastCardPlayed.Color)
                    {
                        isValidPlay = true; // Same color 
                    }
                }
            }
        }

        if (!isValidPlay)
        {
            Debug.Log("Invalid move! You must play a card with the same color or number.");
            return; // Prevent playing an invalid card
        }

        // Remove the previous card from the discard pile (if any)
        if (discardPile.childCount > 0)
        {
            Destroy(discardPile.GetChild(0).gameObject); // Deletes last played card
        }

        // Move new card to the discard pile
        selectedCard.transform.SetParent(discardPile);
        selectedCard.transform.localPosition = Vector3.zero; // Center it
        selectedCard.transform.localScale = Vector3.one * globalScale; // Reset scale

        // Remove from hand
        cardsInHand.RemoveAt(selectedIndex);

        // Update selection index
        selectedIndex = Mathf.Clamp(selectedIndex, 0, cardsInHand.Count - 1);

        // If P1 has cards left, update selection
        if (isPlayerOne && cardsInHand.Count > 0)
        {
            SelectCard(selectedIndex);
        }

        GameManager.Instance.SwitchTurn();
    }

    public void AddCardToHand(GameObject cardObject)
    {
        if (cardsInHand.Count >= 7)
        {
            Debug.Log("Hand is full, can't add more cards!");
            return; // Ensure no more than 7 cards in hand
        }

        cardsInHand.Add(cardObject);

        // Position the new card at the next available spot
        PositionCards();

        // Only P1 selects the first card when a card is added
        if (isPlayerOne && cardsInHand.Count == 1)
        {
            SelectCard(0);
        }
    }

    private void PositionCards()
    {
        // Position cards based on the predefined positions (from the cardPositions array)
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            if (i < cardPositions.Length)
            {
                cardsInHand[i].transform.localPosition = new Vector3(cardPositions[i], 0, 0);
            }
        }
    }

    private void DrawNewCard()
    {
        if (cardsInHand.Count >= 7)
        {
            Debug.Log("Max cards");
            GameManager.Instance.SwitchTurn();
            return;
        }
        // Get the player hand from CardManager or any other relevant class for drawing a card
        if (CardManager.Instance != null)
        {
            CardManager.Instance.DrawCard(transform); // Call CardManager's draw method
        }
    }
}
