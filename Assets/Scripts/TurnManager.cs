using MoonActive.Connect4;
using System;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private PlayerColor openingPlayer;
    [SerializeField] private BasePlayerController[] players;

    [Header("Grid")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ConnectGameGrid connectGameGrid;

    [Header("Prefabs")]
    [SerializeField] private Disk blueDiskPrefab;
    [SerializeField] private Disk redDiskPrefab;

    private int currentPlayerId;

    public static Action<int> OnTurnEnded;

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
        StartGame();
    }

    public void StartGame()
    {
        // Implement turn order logic here with a state machine
    }

    private void HandleColumnClick(int column)
    {
        Disk diskPrefab = (currentPlayerId == 1) ? blueDiskPrefab : redDiskPrefab;
        connectGameGrid.Spawn(diskPrefab, column, 0);
    }

    private void SetOpeningPlayer(PlayerColor playerColor)
    {
        BasePlayerController firstPlayer = players.FirstOrDefault(e => e.Color == playerColor);
        currentPlayerId = firstPlayer.Id;
    }

    private void OnTurnEndedFunction(int playerId)
    {
        print(playerId);
    }
}
