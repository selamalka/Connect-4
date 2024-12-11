using MoonActive.Connect4;
using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private PlayerColor currentPlayer;
    [SerializeField] private PlayerColor openingPlayer;
    [SerializeField] private BasePlayerController[] playerControllers;

    [Header("Grid")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ConnectGameGrid connectGameGrid;

    [Header("Prefabs")]
    [SerializeField] private GameObject humanPlayerControllerPrefab;
    [SerializeField] private GameObject aiPlayerControllerPrefab;
    [SerializeField] private Disk blueDiskPrefab;
    [SerializeField] private Disk redDiskPrefab;

    private bool isTurnInProgress = false;
    private Disk lastSpawnedDisk;
    private int lastColumnFilled;
    private int lastRowFilled;

    private void OnEnable()
    {
        UIManager.OnConfirmPressed += StartGame;
        UIManager.OnRestartPressed += RestartGame;
        connectGameGrid.ColumnClicked += HandleColumnClick;
    }

    private void OnDisable()
    {
        UIManager.OnConfirmPressed -= StartGame;
        UIManager.OnRestartPressed -= RestartGame;
        connectGameGrid.ColumnClicked -= HandleColumnClick;
    }

    private void StartGame(GameMode gameMode)
    {
        SetPlayers(gameMode);
        SetOpeningPlayer(openingPlayer);
        gridManager.InitializeGrid(gameMode); // Assuming this clears the grid and prepares it
    }

    private void SetPlayers(GameMode gameMode)
    {
        GameObject player1Object;
        GameObject player2Object;

        // Instantiate objects based on the game mode
        switch (gameMode)
        {
            case GameMode.PlayerVsPlayer:
                player1Object = Instantiate(humanPlayerControllerPrefab);
                player2Object = Instantiate(humanPlayerControllerPrefab);
                break;

            case GameMode.PlayerVsComputer:
                player1Object = Instantiate(humanPlayerControllerPrefab);
                player2Object = Instantiate(aiPlayerControllerPrefab);
                break;

            case GameMode.ComputerVsComputer:
                player1Object = Instantiate(aiPlayerControllerPrefab);
                player2Object = Instantiate(aiPlayerControllerPrefab);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), "Unsupported game mode.");
        }

        // Fetch the appropriate components
        BasePlayerController player1 = gameMode == GameMode.ComputerVsComputer
            ? player1Object.GetComponent<AIPlayerController>()
            : player1Object.GetComponent<HumanPlayerController>();

        BasePlayerController player2 = gameMode == GameMode.PlayerVsPlayer
            ? player2Object.GetComponent<HumanPlayerController>()
            : player2Object.GetComponent<AIPlayerController>();

        // Set player colors
        player1.SetPlayerColor(PlayerColor.Blue);
        player2.SetPlayerColor(PlayerColor.Red);

        // Assign to the playerControllers array
        playerControllers = new BasePlayerController[2] { player1, player2 };
    }

    public void OnAIMoveChosen(int column)
    {
        // Directly call HandleColumnClick to process the AI's move
        HandleColumnClick(column);
    }

    private void HandleColumnClick(int column)
    {
        if (isTurnInProgress)
        {
            return; // Prevent multiple triggers in the same turn
        }

        isTurnInProgress = true;

        if (gridManager.IsColumnFull(column))
        {
            Debug.Log("Column is full");
            isTurnInProgress = false;
            return;
        }

        // Determine the row where the disk will land
        int row = gridManager.GetNextAvailableRow(column);

        lastRowFilled = row;
        lastColumnFilled = column;

        // Spawn the disk and get the actual instance
        Disk spawnedDisk = (Disk)connectGameGrid.Spawn(GetDiskByPlayerColor(currentPlayer), column, 0);
        lastSpawnedDisk = spawnedDisk;

        if (lastSpawnedDisk != null)
        {
            // Subscribe to the StoppedFalling event of the spawned disk
            lastSpawnedDisk.StoppedFalling += OnStoppedFallingWrapper;
        }
    }

    /*    private void HandleColumnClick(int column)
        {
            if (isTurnInProgress)
            {
                return; // Prevent multiple triggers in the same turn
            }

            isTurnInProgress = true;

            if (gridManager.IsColumnFull(column))
            {
                print("Column is full");
                isTurnInProgress = false;
                return;
            }

            // Determine the row where the disk will land
            int row = gridManager.GetNextAvailableRow(column);

            lastRowFilled = row;
            lastColumnFilled = column;

            // Spawn the disk and get the actual instance
            Disk spawnedDisk = (Disk)connectGameGrid.Spawn(GetDiskByPlayerColor(currentPlayer), column, 0);
            lastSpawnedDisk = spawnedDisk;

            if (lastSpawnedDisk != null)
            {
                // Subscribe to the StoppedFalling event of the spawned disk
                lastSpawnedDisk.StoppedFalling += OnStoppedFallingWrapper;
            }
        }*/

    private void OnStoppedFallingWrapper()
    {
        OnStoppedFalling(lastRowFilled, lastColumnFilled);
    }

    private void OnStoppedFalling(int row, int column)
    {
        // Check for a win after the disk has landed
        if (gridManager.CheckWin(row, column, currentPlayer))
        {
            Debug.Log($"Player {currentPlayer} wins!");
            // Handle win logic (e.g., display a message, end the game)
            return;
        }

        // Check for a draw
        if (gridManager.CheckDraw())
        {
            Debug.Log("It's a draw!");
            // Handle draw logic (e.g., display a message, end the game)
            return;
        }

        EndTurn();
    }

    private void EndTurn()
    {
        SwitchCurrentPlayer();
        isTurnInProgress = false; // Allow the next turn to proceed
        lastSpawnedDisk.StoppedFalling -= OnStoppedFallingWrapper;

        // Call MakeMove for the next player (either human or AI)
        playerControllers.First(player => player.PlayerColor == currentPlayer).MakeMove();
    }

    private void RestartGame()
    {
        gridManager.ClearGrid();
    }

    private void SwitchCurrentPlayer()
    {
        // Find the index of the current player in the array
        int currentIndex = Array.FindIndex(playerControllers, player => player.PlayerColor == currentPlayer);

        // Calculate the next index (wrap around using modulo)
        int nextIndex = (currentIndex + 1) % playerControllers.Length;

        // Update the current player to the next one
        currentPlayer = playerControllers[nextIndex].PlayerColor;
    }

    public Disk GetDiskByPlayerColor(PlayerColor playerColor)
    {
        switch (playerColor)
        {
            case PlayerColor.Blue:
                return blueDiskPrefab;

            case PlayerColor.Red:
                return redDiskPrefab;

            default:
                break;
        }

        return null;
    }

    private void SetOpeningPlayer(PlayerColor playerColor)
    {
        BasePlayerController firstPlayer = playerControllers.FirstOrDefault(e => e.PlayerColor == playerColor);
        currentPlayer = firstPlayer.PlayerColor;
    }
}
