using MoonActive.Connect4;
using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: Header("Players")]
    [field: SerializeField] public PlayerColor CurrentPlayer { get; private set; }
    [SerializeField] private PlayerColor openingPlayer;
    [SerializeField] private BasePlayerController[] playerControllers;

    [Header("Grid")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ConnectGameGrid connectGameGrid;

    [Header("Prefabs")]
    [SerializeField] private Disk blueDiskPrefab;
    [SerializeField] private Disk redDiskPrefab;

    private bool isTurnInProgress = false;

    public static Action<PlayerColor> OnTurnEnded;

    private void OnEnable()
    {
        connectGameGrid.ColumnClicked += HandleColumnClick;
        OnTurnEnded += OnTurnEndedFunction;
    }

    private void OnDisable()
    {
        connectGameGrid.ColumnClicked -= HandleColumnClick;
        OnTurnEnded -= OnTurnEndedFunction;
    }

    private void Start()
    {
        SetOpeningPlayer(openingPlayer);
    }

    private void HandleColumnClick(int column)
    {
        if (isTurnInProgress)
        {
            return; // Prevent multiple triggers in the same turn
        }

        isTurnInProgress = true;

        // Spawn the disk
        Disk diskPrefab = GetDiskByPlayerColor(CurrentPlayer);
        connectGameGrid.Spawn(diskPrefab, column, 0);

        // Trigger turn end after successful spawn
        OnTurnEnded?.Invoke(CurrentPlayer);
    }

    private void OnTurnEndedFunction(PlayerColor playerColor)
    {
        // Check win conditions here (if applicable)
        SwitchCurrentPlayer();
        isTurnInProgress = false; // Allow the next turn to proceed
    }

    private void SwitchCurrentPlayer()
    {
        // Find the index of the current player in the array
        int currentIndex = Array.FindIndex(playerControllers, player => player.PlayerColor == CurrentPlayer);

        // Calculate the next index (wrap around using modulo)
        int nextIndex = (currentIndex + 1) % playerControllers.Length;

        // Update the current player to the next one
        CurrentPlayer = playerControllers[nextIndex].PlayerColor;
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

    public ConnectGameGrid GetConnectGameGrid() => connectGameGrid;

    private void SetOpeningPlayer(PlayerColor playerColor)
    {
        BasePlayerController firstPlayer = playerControllers.FirstOrDefault(e => e.PlayerColor == playerColor);
        CurrentPlayer = firstPlayer.PlayerColor;
    }
}
