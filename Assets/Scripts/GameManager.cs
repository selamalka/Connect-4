using DG.Tweening;
using MoonActive.Connect4;
using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: Header("Game Status")]
    [field: SerializeField] public GameMode GameMode { get; private set; }
    [field: SerializeField] public bool IsGameActive { get; private set; }

    [Header("Players")]
    [SerializeField] private PlayerColor currentPlayer;
    [SerializeField] private PlayerColor openingPlayer;
    [SerializeField] private BasePlayerController[] playerControllers;

    [Header("Grid")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ConnectGameGrid connectGameGrid;
    [SerializeField] private GameObject raycastBlocker;

    [Header("Prefabs")]
    [SerializeField] private GameObject humanPlayerControllerPrefab;
    [SerializeField] private GameObject aiPlayerControllerPrefab;
    [SerializeField] private Disk blueDiskPrefab;
    [SerializeField] private Disk redDiskPrefab;

    private bool isTurnInProgress = false;
    private int lastColumnFilled;
    private int lastRowFilled;
    private Disk lastSpawnedDisk;

    public bool IsGamePaused { get; private set; }

    private void OnEnable()
    {
        UIManager.OnSelectGameMode += SetGameMode;
        UIManager.OnConfirmPressed += StartGame;
        UIManager.OnRestartPressed += RestartGame;
        connectGameGrid.ColumnClicked += HandleColumnClick;
    }

    private void OnDisable()
    {
        UIManager.OnSelectGameMode -= SetGameMode;
        UIManager.OnConfirmPressed -= StartGame;
        UIManager.OnRestartPressed -= RestartGame;
        connectGameGrid.ColumnClicked -= HandleColumnClick;
    }

    private void StartGame(GameMode gameMode)
    {
        raycastBlocker.SetActive(false);

        if (IsGameActive)
        {
            gridManager.ClearGrid();
            isTurnInProgress = false;
            SetPlayers(gameMode);
            SetOpeningPlayer(openingPlayer);
        }
        else
        {
            IsGameActive = true;
            SetPlayers(gameMode);
            SetOpeningPlayer(openingPlayer);
            gridManager.InitializeGrid(gameMode);
        }

        if (GameMode == GameMode.ComputerVsComputer)
        {
            GetPlayerControllerByColor(currentPlayer).MakeMove();
        }
    }

    private void SetPlayers(GameMode gameMode)
    {
        // Destroy any existing players
        DestroyAllPlayers();

        // Instantiate player objects based on game mode
        GameObject player1Object = InstantiatePlayer1Object(gameMode);
        GameObject player2Object = InstantiatePlayer2Object(gameMode);

        // Fetch the appropriate components
        BasePlayerController player1 = GetPlayer1Controller(player1Object, gameMode);
        BasePlayerController player2 = GetPlayer2Controller(player2Object, gameMode);

        // Set GameManager for AI players
        InitializeAIPlayer(player1);
        InitializeAIPlayer(player2);

        // Set player indexes
        player1.SetPlayerIndex(1);
        player2.SetPlayerIndex(2);

        // Set player colors
        player1.SetPlayerColor(PlayerColor.Blue);
        player2.SetPlayerColor(PlayerColor.Red);

        // Assign to the playerControllers array
        playerControllers = new BasePlayerController[2] { player1, player2 };
    }

    private GameObject InstantiatePlayer1Object(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.PlayerVsPlayer:
            case GameMode.PlayerVsComputer:
                return Instantiate(humanPlayerControllerPrefab);

            case GameMode.ComputerVsComputer:
                return Instantiate(aiPlayerControllerPrefab);

            default:
                return null;
        }
    }

    private GameObject InstantiatePlayer2Object(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.PlayerVsPlayer:
                return Instantiate(humanPlayerControllerPrefab);

            case GameMode.PlayerVsComputer:
            case GameMode.ComputerVsComputer:
                return Instantiate(aiPlayerControllerPrefab);

            default:
                return null;
        }
    }

    private BasePlayerController GetPlayer1Controller(GameObject playerObject, GameMode gameMode)
    {
        return gameMode == GameMode.ComputerVsComputer
            ? playerObject.GetComponent<AIPlayerController>()
            : playerObject.GetComponent<HumanPlayerController>();
    }

    private BasePlayerController GetPlayer2Controller(GameObject playerObject, GameMode gameMode)
    {
        return gameMode == GameMode.PlayerVsPlayer
            ? playerObject.GetComponent<HumanPlayerController>()
            : playerObject.GetComponent<AIPlayerController>();
    }

    private void InitializeAIPlayer(BasePlayerController player)
    {
        if (player is AIPlayerController aiPlayer)
        {
            aiPlayer.SetGameManager(this);
        }
    }

    public void DestroyAllPlayers()
    {
        var allPlayer = FindObjectsOfType<BasePlayerController>();
        foreach (var player in allPlayer) { Destroy(player.gameObject); }
    }

    private void SetGameMode(GameMode gameMode)
    {
        GameMode = gameMode;
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

        AudioManager.Instance.PlayAudioWithRandomPitch(AudioType.Game, "Round Click", 0.6f, 1.2f);

        isTurnInProgress = true;
        raycastBlocker.SetActive(true);

        if (gridManager.IsColumnFull(column))
        {
            Debug.Log("Column is full");
            isTurnInProgress = false;
            raycastBlocker.SetActive(false);
            return;
        }

        // Determine the row where the disk will land
        int row = gridManager.GetNextAvailableRow(column);

        lastRowFilled = row;
        lastColumnFilled = column;

        // Spawn the disk and get the actual instance
        Disk spawnedDisk = (Disk)connectGameGrid.Spawn(GetDiskByPlayerColor(currentPlayer), column, 0);

        int randomNumber = UnityEngine.Random.Range(0, 2);
        float randomValue = randomNumber == 0 ? 270f : 90f;

        spawnedDisk.transform.rotation = Quaternion.Euler(0, 0, randomValue);
        spawnedDisk.transform.DORotate(new Vector3(0, 0, 0), 0.3f);

        spawnedDisk.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f).SetEase(Ease.OutQuad);

        lastSpawnedDisk = spawnedDisk;

        if (lastSpawnedDisk != null)
        {
            // Subscribe to the StoppedFalling event of the spawned disk
            lastSpawnedDisk.StoppedFalling += OnStoppedFallingWrapper;
        }
    }

    private void OnStoppedFallingWrapper()
    {
        OnStoppedFalling(lastRowFilled, lastColumnFilled);
    }

    private void OnStoppedFalling(int row, int column)
    {
        AudioManager.Instance.PlayAudio(AudioType.Game, "Disk In Cell");

        // Check for a win after the disk has landed
        if (gridManager.CheckWin(row, column, currentPlayer))
        {
            // Find the index of the winning player by color
            int winningPlayerIndex = GetPlayerControllerByColor(currentPlayer).PlayerIndex;

            RefreshGame();

            AudioManager.Instance.PlayAudio(AudioType.Game, "Victory");
            UIManager.OnAnnouncement?.Invoke($"Player {winningPlayerIndex} Wins!");

            // Handle win logic (e.g., display a message, end the game)
            return;
        }

        // Check for a draw
        if (gridManager.CheckDraw())
        {
            RefreshGame();

            UIManager.OnAnnouncement?.Invoke("It's a Draw!");

            // Handle draw logic (e.g., display a message, end the game)
            return;
        }

        if (IsGameActive)
        {
            EndTurn();
        }
    }

    private void RefreshGame()
    {
        gridManager.ClearGrid();
        IsGameActive = false;
        isTurnInProgress = false;
        playerControllers = null;
    }

    private void EndTurn()
    {
        HandleRaycastBlocker();
        SwitchCurrentPlayer();
        isTurnInProgress = false; // Allow the next turn to proceed
        lastSpawnedDisk.StoppedFalling -= OnStoppedFallingWrapper;
        lastSpawnedDisk.transform.DOPunchScale(new Vector3(-0.2f, -0.2f, -0.2f), 0.2f).SetEase(Ease.OutQuad);

        // Call MakeMove for the next player (either human or AI)
        GetPlayerControllerByColor(currentPlayer).MakeMove();
    }

    private void HandleRaycastBlocker()
    {
        BasePlayerController basePlayerContorller = GetPlayerControllerByColor(currentPlayer);

        if (basePlayerContorller.GetComponent<AIPlayerController>() != null)
        {
            raycastBlocker.SetActive(false);
        }

        if (GameMode == GameMode.PlayerVsPlayer)
        {
            raycastBlocker.SetActive(false);
        }
    }

    private void RestartGame()
    {
        isTurnInProgress = false;
        gridManager.ClearGrid();
        SetPlayers(GameMode);
        IsGameActive = true;
        raycastBlocker.SetActive(false);

        if (GameMode == GameMode.ComputerVsComputer)
        {
            GetPlayerControllerByColor(currentPlayer).MakeMove();
        }
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

    private BasePlayerController GetPlayerControllerByColor(PlayerColor playerColor)
    {
        return playerControllers.FirstOrDefault(e => e.PlayerColor == playerColor);
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
        BasePlayerController firstPlayer = GetPlayerControllerByColor(playerColor);
        currentPlayer = firstPlayer.PlayerColor;
    }

    public void SetIsGamePaused(bool value)
    {
        IsGamePaused = value;

        if (GameMode == GameMode.ComputerVsComputer && value == true)
        {
            GetPlayerControllerByColor(currentPlayer).MakeMove();
        }
    }
}
