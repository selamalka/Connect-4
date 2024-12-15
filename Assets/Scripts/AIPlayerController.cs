using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inherits from the BasePlayerController class, handles AI logic separately from the Human logic
/// </summary>
public class AIPlayerController : BasePlayerController
{
    [SerializeField] private DifficultyMode difficulty = DifficultyMode.Easy; // Set AI difficulty
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

        int chosenColumn = -1;

        // Select AI logic based on difficulty
        switch (difficulty)
        {
            case DifficultyMode.Easy:
                chosenColumn = GetRandomValidColumn();
                break;

            case DifficultyMode.Medium:
                chosenColumn = GetMediumDifficultyColumn();
                break;

            case DifficultyMode.Hard:
                chosenColumn = GetBestColumn();
                break;
        }

        if (chosenColumn == -1)
        {
            Debug.LogError("AI could not find a valid move!");
            yield break;
        }

        Debug.Log($"{PlayerColor} (AI) chooses column {chosenColumn}");

        // Inform the GameManager of the chosen column
        gameManager.OnAIMoveChosen(chosenColumn);
    }

    private int GetMediumDifficultyColumn()
    {
        PlayerColor opponent = PlayerColor == PlayerColor.Blue ? PlayerColor.Red : PlayerColor.Blue;

        // Medium AI only blocks winning moves or defaults to random
        int blockingColumn = GetStrategicColumn(PlayerColor, opponent);
        if (blockingColumn != -1)
        {
            return blockingColumn;
        }

        return GetRandomValidColumn(); // Default fallback
    }

    private int GetBestColumn()
    {
        // Try to win
        int winningColumn = GetWinningColumn(PlayerColor);
        if (winningColumn != -1)
        {
            return winningColumn;
        }

        // Try to block opponent
        PlayerColor opponent = PlayerColor == PlayerColor.Blue ? PlayerColor.Red : PlayerColor.Blue;
        int blockingColumn = GetStrategicColumn(PlayerColor, opponent);
        if (blockingColumn != -1)
        {
            return blockingColumn;
        }

        // Use heuristic
        int heuristicColumn = GetHeuristicColumn();
        if (heuristicColumn != -1)
        {
            return heuristicColumn;
        }

        // Default to random
        return GetRandomValidColumn();
    }

    private int GetWinningColumn(PlayerColor player)
    {
        for (int column = 0; column < gridManager.Columns; column++)
        {
            if (!gridManager.IsColumnFull(column))
            {
                int row = gridManager.GetNextAvailableRow(column);

                // Simulate placing a disk for the AI
                gridManager.GetCell(row, column).SetPlayerInCell(player);

                if (gridManager.CheckWin(row, column, player))
                {
                    // Undo the simulation
                    gridManager.GetCell(row, column).SetPlayerInCell(PlayerColor.None);
                    return column; // Take the winning move
                }

                // Undo the simulation
                gridManager.GetCell(row, column).SetPlayerInCell(PlayerColor.None);
            }
        }

        return -1; // No winning move found
    }

    private int GetStrategicColumn(PlayerColor currentPlayer, PlayerColor opponent)
    {
        for (int column = 0; column < gridManager.Columns; column++)
        {
            if (!gridManager.IsColumnFull(column))
            {
                int row = gridManager.GetNextAvailableRow(column);

                // Simulate placing a disk for the opponent
                gridManager.GetCell(row, column).SetPlayerInCell(opponent);

                if (gridManager.CheckWin(row, column, opponent))
                {
                    // Undo the simulation
                    gridManager.GetCell(row, column).SetPlayerInCell(PlayerColor.None);
                    return column; // Block this column
                }

                // Undo the simulation
                gridManager.GetCell(row, column).SetPlayerInCell(PlayerColor.None);
            }
        }

        return -1; // No immediate threat found
    }

    private int GetHeuristicColumn()
    {
        int[] columnPriority = { 3, 2, 4, 1, 5, 0, 6 };

        foreach (int column in columnPriority)
        {
            if (!gridManager.IsColumnFull(column))
            {
                return column;
            }
        }

        return -1; // Default fallback
    }

    private int GetRandomValidColumn()
    {
        var validColumns = new List<int>();

        for (int column = 0; column < gridManager.Columns; column++)
        {
            if (!gridManager.IsColumnFull(column))
            {
                validColumns.Add(column);
            }
        }

        return validColumns.Count > 0 ? validColumns[Random.Range(0, validColumns.Count)] : -1;
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void SetDifficulty(DifficultyMode difficulty)
    {
        this.difficulty = difficulty;
    }
}
