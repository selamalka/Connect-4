using MoonActive.Connect4;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private BasePlayerController[] players;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ConnectGameGrid connectGameGrid;
    [SerializeField] private Disk player1DiskPrefab;
    [SerializeField] private Disk player2DiskPrefab;

    private int currentPlayer;

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
        SetOpeningPlayer(PlayerColor.Blue);
        StartGame();
    }

    private void SetOpeningPlayer(PlayerColor playerColor)
    {
        BasePlayerController firstPlayer = players.FirstOrDefault(e => e.Color == playerColor);
        currentPlayer = firstPlayer.Id;
    }

    public void StartGame()
    {
        StartTurn();
    }

    private void StartTurn()
    {

    }

    private void HandleColumnClick(int column)
    {
        Disk diskPrefab = (currentPlayer == 1) ? player1DiskPrefab : player2DiskPrefab;
        connectGameGrid.Spawn(diskPrefab, column, 0);
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
}
