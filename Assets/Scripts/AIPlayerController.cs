using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inherits from the BasePlayerController class, handles AI logic separately from the Human logic
/// </summary>
public class AIPlayerController : BasePlayerController
{
    private GameManager gameManager;
    private GridManager gridManager;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    public override void MakeMove()
    {
        StartCoroutine(PerformAIMove());
    }

    private IEnumerator PerformAIMove()
    {        
        // Wait while the game is paused
        while (gameManager.IsGamePaused)
        {
            yield return null; // Wait for the next frame
        }

        // Simulate "thinking time" for AI
        yield return new WaitForSeconds(0.6f);

        // Simple AI logic: Pick the first available column
        int chosenColumn = GetRandomValidColumn();

        if (chosenColumn == -1)
        {
            Debug.LogError("AI could not find a valid move!");
            yield break;
        }

        Debug.Log($"{PlayerColor} (AI) chooses column {chosenColumn}");

        // Inform the GameManager of the chosen column
        gameManager.OnAIMoveChosen(chosenColumn);
    }

    private int GetRandomValidColumn()
    {
        // Find all columns with available moves
        var validColumns = new List<int>();

        for (int column = 0; column < gridManager.Columns; column++)
        {
            if (!gridManager.IsColumnFull(column))
            {
                validColumns.Add(column);
            }
        }

        // Return a random valid column, or -1 if none are available
        return validColumns.Count > 0 ? validColumns[Random.Range(0, validColumns.Count)] : -1;
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
}
