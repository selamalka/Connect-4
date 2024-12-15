using DG.Tweening;
using MoonActive.Connect4;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Manages the core functionality of the game
/// </summary>
public class GameManager : MonoBehaviour
{
    [field: Header("Game Status")]
    [field: SerializeField] public GameMode GameMode { get; private set; }
    [field: SerializeField] public bool IsGameActive { get; private set; }
    [field: SerializeField] public bool IsGamePaused { get; private set; }

    [field: Header("Players")]
    [field: SerializeField] public PlayerColor CurrentPlayer { get; private set; }
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
    
    public static Action<PlayerColor> OnCurrentPlayerChanged;
    
    private SettingsManager settingsManager;    

    private void OnEnable()
    {
        UIManager.OnSelectGameMode += SetGameMode;
        UIManager.OnConfirmPressed += StartGame;
        UIManager.OnRestart += RestartGame;

        if (connectGameGrid != null)
        {
            connectGameGrid.ColumnClicked += HandleColumnClick;
        }
    }

    private void OnDisable()
    {
        UIManager.OnSelectGameMode -= SetGameMode;
        UIManager.OnConfirmPressed -= StartGame;
        UIManager.OnRestart -= RestartGame;

        if (connectGameGrid != null)
        {
            connectGameGrid.ColumnClicked -= HandleColumnClick;
        }
    }

    private void Start()
    {
        if (FindObjectOfType<SettingsManager>() != null)
        {
            settingsManager = FindObjectOfType<SettingsManager>();
        }
    }

    private void StartGame(GameMode gameMode)
    {
        // Set up the game depending on the selected mode
        if (GameMode != GameMode.ComputerVsComputer)
        {
            raycastBlocker.SetActive(false); // Enable player input if not AI vs AI
        }

        if (IsGameActive)
        {
            // If a game is already in progress, reset the grid and players
            gridManager.ClearGrid();
            isTurnInProgress = false;
            SetPlayers(gameMode); // Initialize player controllers
            SetOpeningPlayer(openingPlayer); // Set the starting player
        }
        else
        {
            // If starting a new game, initialize game state and grid
            IsGameActive = true;
            SetPlayers(gameMode); // Initialize player controllers
            SetOpeningPlayer(openingPlayer); // Set the starting player
            gridManager.InitializeGrid(gameMode); // Prepare the grid
        }

        // Trigger the first move for AI if the mode is AI vs AI
        if (GameMode == GameMode.ComputerVsComputer)
        {
            GetPlayerControllerByColor(CurrentPlayer).MakeMove();
        }
    }
    private void SwitchCurrentPlayer()
    {
        // Determine the index of the current player in the controllers array
        int currentIndex = Array.FindIndex(playerControllers, player => player.PlayerColor == CurrentPlayer);

        // Calculate the next player's index using modulo for wrapping
        int nextIndex = (currentIndex + 1) % playerControllers.Length;

        // Update the current player to the next one
        CurrentPlayer = playerControllers[nextIndex].PlayerColor;

        // Notify any listeners about the player change
        OnCurrentPlayerChanged?.Invoke(CurrentPlayer);
    }
    private void RefreshGame()
    {
        // Reset the grid and game state for a fresh start
        gridManager.ClearGrid();
        IsGameActive = false;
        isTurnInProgress = false;
        playerControllers = null;
    }
    private void RestartGame()
    {
        // Reset the game while keeping the selected game mode
        isTurnInProgress = false;
        gridManager.ClearGrid();
        SetPlayers(GameMode);
        IsGameActive = true;

        // Handle input blocking for AI vs AI
        if (GameMode == GameMode.ComputerVsComputer)
        {
            raycastBlocker.SetActive(true);
            GetPlayerControllerByColor(CurrentPlayer).MakeMove(); // Trigger AI move
        }
        else
        {
            raycastBlocker.SetActive(false);
        }
    }
    private void EndTurn()
    {
        // Finalize the current turn and prepare for the next
        HandleRaycastBlocker(); // Adjust input handling based on game mode
        SwitchCurrentPlayer();
        isTurnInProgress = false;

        // Unsubscribe from the last disk's events
        lastSpawnedDisk.StoppedFalling -= OnStoppedFallingWrapper;

        // Apply animation to the last placed disk
        lastSpawnedDisk.transform.DOPunchScale(new Vector3(-0.2f, -0.2f, -0.2f), 0.2f).SetEase(Ease.OutQuad);

        // Prompt the next player (human or AI) to make their move
        GetPlayerControllerByColor(CurrentPlayer).MakeMove();
    }
    public void DestroyAllPlayers()
    {
        // Remove all player controller objects from the scene
        var allPlayers = FindObjectsOfType<BasePlayerController>();
        foreach (var player in allPlayers)
        {
            Destroy(player.gameObject);
        }
    }

    private void HandleColumnClick(int column)
    {
        // Prevent the player from interacting again if a turn is already in progress
        if (isTurnInProgress)
        {
            return;
        }

        isTurnInProgress = true; // Mark the turn as in progress

        // Check if the selected column is full
        if (gridManager.IsColumnFull(column))
        {
            AudioManager.Instance.PlayAudio(AudioType.Game, "Column Full");
            isTurnInProgress = false; // Reset turn progress to allow another attempt
            raycastBlocker.SetActive(false); // Re-enable input for the player
            return;
        }
        else
        {
            raycastBlocker.SetActive(true); // Block additional input while processing the current turn
            AudioManager.Instance.PlayAudioWithRandomPitch(AudioType.Game, "Round Click", 0.6f, 1.2f);
        }

        // Determine the first available row in the selected column
        int row = gridManager.GetNextAvailableRow(column);

        // Store the position of the disk placement for later use
        lastRowFilled = row;
        lastColumnFilled = column;

        // Spawn a disk for the current player in the specified column
        Disk spawnedDisk = (Disk)connectGameGrid.Spawn(GetDiskByPlayerColor(CurrentPlayer), column, 0);

        // Apply a random rotation to the disk for visual variety
        int randomNumber = UnityEngine.Random.Range(0, 2);
        float randomValue = randomNumber == 0 ? 270f : 90f;

        spawnedDisk.transform.rotation = Quaternion.Euler(0, 0, randomValue);
        spawnedDisk.transform.DORotate(new Vector3(0, 0, 0), 0.3f);

        // Add animation for a popping effect
        spawnedDisk.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), 0.2f).SetEase(Ease.OutQuad);

        lastSpawnedDisk = spawnedDisk; // Keep a reference to the last spawned disk

        // Subscribe to the disk's StoppedFalling event to handle further actions once the disk settles
        if (lastSpawnedDisk != null)
        {
            lastSpawnedDisk.StoppedFalling += OnStoppedFallingWrapper;
        }
    }
    private void HandleRaycastBlocker()
    {
        // Get the controller for the current player
        BasePlayerController currentPlayerController = GetPlayerControllerByColor(CurrentPlayer);

        // Check if the current player is an AI and disable the raycast blocker if true
        if (currentPlayerController.GetComponent<AIPlayerController>() != null)
        {
            raycastBlocker.SetActive(false);
        }

        // If the game is Player vs Player, disable the raycast blocker to allow player input
        if (GameMode == GameMode.PlayerVsPlayer)
        {
            raycastBlocker.SetActive(false);
        }

        // If the game is AI vs AI, enable the raycast blocker to prevent player input
        if (GameMode == GameMode.ComputerVsComputer)
        {
            raycastBlocker.SetActive(true);
        }
    }
    private void HandleDraw()
    {
        RefreshGame(); // Reset the game state to its initial setup
        UIManager.OnAnnouncement?.Invoke("It's a Draw!");
    }
    private async Task HandleWin()
    {
        int winningPlayerIndex = GetPlayerControllerByColor(CurrentPlayer).PlayerIndex; // Get the index of the winning player

        RefreshGame(); // Reset the game state for a potential new game

        // Temporarily reduce the music volume to emphasize the victory announcement
        float currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume");

        settingsManager.SetMusicVolumeBySettings(currentMusicVolume * 0.65f);

        AudioManager.Instance.PlayAudio(AudioType.Game, "Victory");
        UIManager.OnAnnouncement?.Invoke($"Player {winningPlayerIndex} Wins!");

        await Task.Delay(2500); // Pause for 2.5 seconds to let the announcement display fully

        settingsManager.SetMusicVolumeBySettings(currentMusicVolume);
    }

    private bool CheckWinCondition(int row, int column)
    {
        // Delegates the win-checking logic to the GridManager.
        // Parameters:
        // - row: The row index of the most recently placed disk.
        // - column: The column index of the most recently placed disk.
        // Returns:
        // - True if the current player has achieved a win condition (e.g., four in a row);
        //   otherwise, false.
        return gridManager.CheckWin(row, column, CurrentPlayer);
    }
    private bool CheckDrawCondition()
    {
        // Delegates the draw-checking logic to the GridManager.
        // Returns:
        // - True if the board is full and no more moves can be made, resulting in a draw;
        //   otherwise, false.
        return gridManager.CheckDraw();
    }

    public void OnAIMoveChosen(int column)
    {
        // Triggered when the AI selects a column to play in.
        // Directly processes the AI's move by calling the same logic used for human moves.
        HandleColumnClick(column);
    }
    private void OnStoppedFallingWrapper()
    {
        // A wrapper method that ensures the OnStoppedFalling method is called with the
        // correct row and column of the most recently placed disk.
        OnStoppedFalling(lastRowFilled, lastColumnFilled);
    }
    private async void OnStoppedFalling(int row, int column)
    {
        // Called when a disk finishes its fall and settles into a cell.

        AudioManager.Instance.PlayAudio(AudioType.Game, "Disk In Cell");

        // Check if the last move caused a win condition
        if (CheckWinCondition(row, column))
        {
            await HandleWin(); // Handle the win process
            return;
        }

        // Check if the board is full, resulting in a draw
        if (CheckDrawCondition())
        {
            HandleDraw(); // Handle the draw process
            return;
        }

        // If the game is still active, proceed to the next turn
        if (IsGameActive)
        {
            EndTurn(); // Switch players and continue the game
        }
    }

    private GameObject InstantiatePlayer1Object(GameMode gameMode)
    {
        // Instantiate the appropriate prefab for Player 1 based on the game mode
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
        // Instantiate the appropriate prefab for Player 2 based on the game mode
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
    private void InitializeAIPlayer(BasePlayerController player)
    {
        // If the player is an AI, set its reference to the GameManager
        if (player is AIPlayerController aiPlayer)
        {
            aiPlayer.SetGameManager(this);
        }
    }

    private BasePlayerController GetPlayer1Controller(GameObject playerObject, GameMode gameMode)
    {
        // Return the appropriate controller type for Player 1 based on the game mode
        return gameMode == GameMode.ComputerVsComputer
            ? playerObject.GetComponent<AIPlayerController>()
            : playerObject.GetComponent<HumanPlayerController>();
    }
    private BasePlayerController GetPlayer2Controller(GameObject playerObject, GameMode gameMode)
    {
        // Return the appropriate controller type for Player 2 based on the game mode
        return gameMode == GameMode.PlayerVsPlayer
            ? playerObject.GetComponent<HumanPlayerController>()
            : playerObject.GetComponent<AIPlayerController>();
    }
    public BasePlayerController GetPlayerControllerByColor(PlayerColor playerColor)
    {
        // Find and return the player controller matching the specified player color
        return playerControllers.FirstOrDefault(e => e.PlayerColor == playerColor);
    }
    public Disk GetDiskByPlayerColor(PlayerColor playerColor)
    {
        // Return the disk prefab corresponding to the given player color
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

    private void SetPlayers(GameMode gameMode)
    {
        // Destroy any existing player objects
        DestroyAllPlayers();

        // Instantiate player objects based on the selected game mode
        GameObject player1Object = InstantiatePlayer1Object(gameMode);
        GameObject player2Object = InstantiatePlayer2Object(gameMode);

        // Get the appropriate controllers for each player
        BasePlayerController player1 = GetPlayer1Controller(player1Object, gameMode);
        BasePlayerController player2 = GetPlayer2Controller(player2Object, gameMode);

        // Initialize AI players by setting the GameManager reference
        InitializeAIPlayer(player1);
        InitializeAIPlayer(player2);

        // Assign player indexes for identification
        player1.SetPlayerIndex(1);
        player2.SetPlayerIndex(2);

        // Assign player colors
        player1.SetPlayerColor(PlayerColor.Blue);
        player2.SetPlayerColor(PlayerColor.Red);

        // Store the controllers in the playerControllers array
        playerControllers = new BasePlayerController[2] { player1, player2 };
    }
    private void SetGameMode(GameMode gameMode)
    {
        GameMode = gameMode;
    }
    private void SetOpeningPlayer(PlayerColor playerColor)
    {
        // Set the first player to the specified opening player color
        BasePlayerController firstPlayer = GetPlayerControllerByColor(playerColor);
        CurrentPlayer = firstPlayer.PlayerColor;
        OnCurrentPlayerChanged?.Invoke(CurrentPlayer);
    }
    public void SetIsGamePaused(bool value)
    {
        IsGamePaused = value;

        // If the game is AI vs AI and is being paused, let the current AI make its move
        if (GameMode == GameMode.ComputerVsComputer && value == true)
        {
            GetPlayerControllerByColor(CurrentPlayer).MakeMove();
        }
    }
}
