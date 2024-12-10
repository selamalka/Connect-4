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
        Disk diskPrefab = (CurrentPlayer == PlayerColor.Blue) ? blueDiskPrefab : redDiskPrefab;
        connectGameGrid.Spawn(diskPrefab, column, 0);
    }

    private void SetOpeningPlayer(PlayerColor playerColor)
    {
        BasePlayerController firstPlayer = playerControllers.FirstOrDefault(e => e.PlayerColor == playerColor);
        CurrentPlayer = firstPlayer.PlayerColor;
    }

    private void OnTurnEndedFunction(PlayerColor playerColor)
    {
        // Check win condition

        // Switch player if the game continues
        SwitchCurrentPlayer();
    }

    private void SwitchCurrentPlayer()
    {
        // Find the index of the current player in the array
        int currentIndex = Array.FindIndex(playerControllers, player => player.PlayerColor == CurrentPlayer);

        // Calculate the next index (wrap around using modulo)
        int nextIndex = (currentIndex + 1) % playerControllers.Length;
        print(nextIndex);
        // Update the current player to the next one
        CurrentPlayer = playerControllers[nextIndex].PlayerColor;

        Debug.Log($"Current player switched to: {CurrentPlayer}");
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
                return null;
        }
    }

    public ConnectGameGrid GetConnectGameGrid() { return connectGameGrid; }    
}
