using MoonActive.Connect4;
using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private BasePlayerController player1;
    [SerializeField] private BasePlayerController player2;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ConnectGameGrid connectGameGrid;
    [SerializeField] private Disk player1DiskPrefab;
    [SerializeField] private Disk player2DiskPrefab;

    private int currentPlayer = 1;

    private void OnEnable()
    {
        connectGameGrid.ColumnClicked += HandleColumnClick;
    }

    private void OnDisable()
    {
        connectGameGrid.ColumnClicked -= HandleColumnClick;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        StartTurn();
    }

    private void StartTurn()
    {
        if (currentPlayer == 1)
        {
            player1.MakeMove();
        }
        else
        {
            player2.MakeMove();
        }
    }

    private void HandleColumnClick(int column)
    {
        Disk diskPrefab = (currentPlayer == 1) ? player1DiskPrefab : player2DiskPrefab;
        // Instantiate the disk at the correct column and row (implement this based on your grid layout)
        connectGameGrid.Spawn(diskPrefab, column, 0);
        gridManager.UpdateGridState(0, column, currentPlayer); // Example: Update grid with the disk placement
    }

    /*    private void HandleMove(int column)
        {
            if (gridManager.IsColumnAvailable(column))
            {
                // Handle disk placement (animation, update grid, etc.)
                PlaceDisk(column);

                if (CheckWinCondition())
                {
                    Debug.Log($"Player {currentPlayer} wins!");
                    // Handle win UI
                }
                else if (gridManager.IsBoardFull())
                {
                    Debug.Log("It's a draw!");
                    // Handle draw UI
                }
                else
                {
                    // Switch turns
                    currentPlayer = (currentPlayer == 1) ? 2 : 1;
                    StartTurn();
                }
            }
            else
            {
                Debug.Log("Column is full! Choose a different column.");
                StartTurn();
            }
        }*/

    private void PlaceDisk(int column)
    {
        Disk diskPrefab = (currentPlayer == 1) ? player1DiskPrefab : player2DiskPrefab;
        // Instantiate the disk at the correct column and row (implement this based on your grid layout)
        connectGameGrid.Spawn(diskPrefab, column, 0);
        gridManager.UpdateGridState(0, column, currentPlayer); // Example: Update grid with the disk placement
    }

    private bool CheckWinCondition()
    {
        // Implement your win-checking logic based on grid state
        return false; // Placeholder
    }
}
