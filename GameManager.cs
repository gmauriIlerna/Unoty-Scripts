using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsPlayerOneTurn { get; private set; } = true; // Must be a field in GameManager


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
}
