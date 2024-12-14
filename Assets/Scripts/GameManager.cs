using DG.Tweening;
using MoonActive.Connect4;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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
        if (GameMode != GameMode.ComputerVsComputer)
        {
            raycastBlocker.SetActive(false);
        }

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
            GetPlayerControllerByColor(CurrentPlayer).MakeMove();
        }
    }
    private void SwitchCurrentPlayer()
    {
        // Find the index of the current player in the array
        int currentIndex = Array.FindIndex(playerControllers, player => player.PlayerColor == CurrentPlayer);

        // Calculate the next index (wrap around using modulo)
        int nextIndex = (currentIndex + 1) % playerControllers.Length;

        // Update the current player to the next one
        CurrentPlayer = playerControllers[nextIndex].PlayerColor;
        OnCurrentPlayerChanged?.Invoke(CurrentPlayer);
    }
    private void RefreshGame()
    {
        gridManager.ClearGrid();
        IsGameActive = false;
        isTurnInProgress = false;
        playerControllers = null;
    }
    private void RestartGame()
    {
        isTurnInProgress = false;
        gridManager.ClearGrid();
        SetPlayers(GameMode);
        IsGameActive = true;

        if (GameMode == GameMode.ComputerVsComputer)
        {
            raycastBlocker.SetActive(true);
            GetPlayerControllerByColor(CurrentPlayer).MakeMove();
        }
        else
        {
            raycastBlocker.SetActive(false);
        }
    }
    private void EndTurn()
    {
        HandleRaycastBlocker();
        SwitchCurrentPlayer();
        isTurnInProgress = false; // Allow the next turn to proceed
        lastSpawnedDisk.StoppedFalling -= OnStoppedFallingWrapper;
        lastSpawnedDisk.transform.DOPunchScale(new Vector3(-0.2f, -0.2f, -0.2f), 0.2f).SetEase(Ease.OutQuad);

        // Call MakeMove for the next player (either human or AI)
        GetPlayerControllerByColor(CurrentPlayer).MakeMove();
    }
    public void DestroyAllPlayers()
    {
        var allPlayer = FindObjectsOfType<BasePlayerController>();
        foreach (var player in allPlayer) { Destroy(player.gameObject); }
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
            AudioManager.Instance.PlayAudio(AudioType.Game, "Column Full");
            isTurnInProgress = false;
            raycastBlocker.SetActive(false);
            return;
        }
        else
        {
            raycastBlocker.SetActive(true);
            AudioManager.Instance.PlayAudioWithRandomPitch(AudioType.Game, "Round Click", 0.6f, 1.2f);
        }

        // Determine the row where the disk will land
        int row = gridManager.GetNextAvailableRow(column);

        lastRowFilled = row;
        lastColumnFilled = column;

        // Spawn the disk and get the actual instance
        Disk spawnedDisk = (Disk)connectGameGrid.Spawn(GetDiskByPlayerColor(CurrentPlayer), column, 0);

        int randomNumber = UnityEngine.Random.Range(0, 2);
        float randomValue = randomNumber == 0 ? 270f : 90f;

        spawnedDisk.transform.rotation = Quaternion.Euler(0, 0, randomValue);
        spawnedDisk.transform.DORotate(new Vector3(0, 0, 0), 0.3f);

        spawnedDisk.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), 0.2f).SetEase(Ease.OutQuad);

        lastSpawnedDisk = spawnedDisk;

        if (lastSpawnedDisk != null)
        {
            // Subscribe to the StoppedFalling event of the spawned disk
            lastSpawnedDisk.StoppedFalling += OnStoppedFallingWrapper;
        }
    }
    private void HandleRaycastBlocker()
    {
        BasePlayerController currentPlayerController = GetPlayerControllerByColor(CurrentPlayer);

        if (currentPlayerController.GetComponent<AIPlayerController>() != null)
        {
            raycastBlocker.SetActive(false);
        }

        if (GameMode == GameMode.PlayerVsPlayer)
        {
            raycastBlocker.SetActive(false);
        }

        if (GameMode == GameMode.ComputerVsComputer)
        {
            raycastBlocker.SetActive(true);
        }
    }
    private void HandleDraw()
    {
        RefreshGame();
        UIManager.OnAnnouncement?.Invoke("It's a Draw!");
    }
    private async Task HandleWin()
    {
        int winningPlayerIndex = GetPlayerControllerByColor(CurrentPlayer).PlayerIndex;

        RefreshGame();

        float currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        settingsManager.SetMusicVolumeBySettings(currentMusicVolume * 0.65f);

        AudioManager.Instance.PlayAudio(AudioType.Game, "Victory");
        UIManager.OnAnnouncement?.Invoke($"Player {winningPlayerIndex} Wins!");

        await Task.Delay(2500);

        settingsManager.SetMusicVolumeBySettings(currentMusicVolume);
    }

    private bool CheckWinCondition(int row, int column)
    {
        return gridManager.CheckWin(row, column, CurrentPlayer);
    }
    private bool CheckDrawCondition()
    {
        return gridManager.CheckDraw();
    }

    public void OnAIMoveChosen(int column)
    {
        // Directly call HandleColumnClick to process the AI's move
        HandleColumnClick(column);
    }
    private void OnStoppedFallingWrapper()
    {
        OnStoppedFalling(lastRowFilled, lastColumnFilled);
    }
    private async void OnStoppedFalling(int row, int column)
    {
        AudioManager.Instance.PlayAudio(AudioType.Game, "Disk In Cell");

        if (CheckWinCondition(row, column))
        {
            await HandleWin();
            return;
        }

        if (CheckDrawCondition())
        {
            HandleDraw();
            return;
        }

        if (IsGameActive)
        {
            EndTurn();
        }
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
    private void InitializeAIPlayer(BasePlayerController player)
    {
        if (player is AIPlayerController aiPlayer)
        {
            aiPlayer.SetGameManager(this);
        }
    }

    private BasePlayerController GetPlayer2Controller(GameObject playerObject, GameMode gameMode)
    {
        return gameMode == GameMode.PlayerVsPlayer
            ? playerObject.GetComponent<HumanPlayerController>()
            : playerObject.GetComponent<AIPlayerController>();
    }
    private BasePlayerController GetPlayer1Controller(GameObject playerObject, GameMode gameMode)
    {
        return gameMode == GameMode.ComputerVsComputer
            ? playerObject.GetComponent<AIPlayerController>()
            : playerObject.GetComponent<HumanPlayerController>();
    }
    public BasePlayerController GetPlayerControllerByColor(PlayerColor playerColor)
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
    private void SetGameMode(GameMode gameMode)
    {
        GameMode = gameMode;
    }
    private void SetOpeningPlayer(PlayerColor playerColor)
    {
        BasePlayerController firstPlayer = GetPlayerControllerByColor(playerColor);
        CurrentPlayer = firstPlayer.PlayerColor;
        OnCurrentPlayerChanged?.Invoke(CurrentPlayer);
    }
    public void SetIsGamePaused(bool value)
    {
        IsGamePaused = value;

        if (GameMode == GameMode.ComputerVsComputer && value == true)
        {
            GetPlayerControllerByColor(CurrentPlayer).MakeMove();
        }
    }
}
