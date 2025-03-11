using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsPlayerOneTurn { get; private set; } = true; // Must be a field in GameManager
    public List<PlayerHand> hands;

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

    public void SwitchTurn()
    {
        IsPlayerOneTurn = !IsPlayerOneTurn;

        if (!IsPlayerOneTurn)
        {
            // AI's turn
            StartCoroutine(SimpleAI.Instance.AIPlayTurn());
        }
    }

    public void checkWinner()
    {
        foreach (PlayerHand hand in hands)
        {
            if (hand.cardsInHand.Count == 0)
            {
                Debug.Log("End game");
                SceneManager.LoadScene("EndGame");
            }
        }
    }
}
