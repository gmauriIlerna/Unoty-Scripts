using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class SimpleAI : MonoBehaviour
{
    public static SimpleAI Instance { get; private set; }

    public PlayerHand aiHand;
    public Transform discardPile;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(AIPlayTurn());
    }

    public void StartAITurn()
    {
        StartCoroutine(AIPlayTurn());
    }

    public IEnumerator AIPlayTurn()
    {
        if (GameManager.Instance.IsPlayerOneTurn)
        {
            yield break; // Prevent running when it's the player's turn
        }

        yield return new WaitForSeconds(1.5f); // Simulate AI thinking
        PlayValidCard();
    }


    private void PlayValidCard()
    {
        PlayerHand playerHand = this.GetComponent<PlayerHand>();
        Debug.Log("Cards in hand: " + playerHand.cardsInHand.Count);
        if (!GameManager.Instance.IsPlayerOneTurn)
        {
            if (aiHand.cardsInHand.Count == 0) return;

            Card lastCardPlayed = null;
            if (discardPile.childCount > 0)
            {
                lastCardPlayed = discardPile.GetChild(0).GetComponent<CardData>().card;
            }

            GameObject cardToPlay = null;
            CardData selectedCardData = null;

            // Find a valid card to play
            foreach (GameObject cardObject in aiHand.cardsInHand)
            {
                CardData cardData = cardObject.GetComponent<CardData>();
                if (cardData == null) continue;

                if (IsValidPlay(cardData.card, lastCardPlayed))
                {
                    cardToPlay = cardObject;
                    selectedCardData = cardData;
                    break;
                }
            }

            // If AI found a valid card, play it
            if (cardToPlay != null)
            {
                // Remove the previous card from the discard pile
                if (discardPile.childCount > 0)
                {
                    Destroy(discardPile.GetChild(0).gameObject);
                }

                // Remove from AI hand
                aiHand.cardsInHand.Remove(cardToPlay);

                // Move card to discard pile
                cardToPlay.transform.SetParent(discardPile);
                cardToPlay.transform.localPosition = Vector3.zero;
                cardToPlay.transform.localScale = Vector3.one * 30f; // Reset scale

                Debug.Log("AI played: " + selectedCardData.card.Type + " " + selectedCardData.card.Color);

                GameManager.Instance.checkWinner();
                // Switch turn back to player
                if (selectedCardData.card.Type == CardType.Skip || selectedCardData.card.Type == CardType.Reverse)
                {
                    GameManager.Instance.SwitchTurn();
                }

                GameManager.Instance.SwitchTurn();
            }
            else
            {
                Debug.Log("AI has no valid cards to play!");
                // (Optional) Here you could implement a "draw card" action for the AI.
                if(playerHand.cardsInHand.Count < 7)
                {
                    DrawNewCard();
                }
                GameManager.Instance.SwitchTurn();
            }
        }
    }


    private bool IsValidPlay(Card playedCard, Card lastCardPlayed)
    {
        if (lastCardPlayed == null) return true;

        if (playedCard.Type == Card.CardType.Wild || playedCard.Type == Card.CardType.Wild_Draw)
        {
            return true;
        }
        else
        {

            if (lastCardPlayed.Type == Card.CardType.Wild || lastCardPlayed.Type == Card.CardType.Wild_Draw)
            {
                return true;
            }
            else
            {

                if (playedCard.Type == Card.CardType.Number)
                {
                    return (playedCard.Color == lastCardPlayed.Color) || // Same color
                                   (playedCard.Number == lastCardPlayed.Number); ;
                }
                else if (playedCard.Type == lastCardPlayed.Type || playedCard.Color == lastCardPlayed.Color)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DrawNewCard()
    {
        PlayerHand playerHand = this.GetComponent<PlayerHand>();
        if (playerHand.cardsInHand.Count >= 7)
        {
            Debug.Log("Max cards");
            return;
        }
        // Get the player hand from CardManager or any other relevant class for drawing a card
        if (CardManager.Instance != null)
        {
            CardManager.Instance.DrawCard(transform); // Call CardManager's draw method
        }
    }
}
