using MoonActive.Connect4;
using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private PlayerColor openingPlayer;
    [SerializeField] private PlayerColor currentPlayer;
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
        StartGame();
    }

    public void StartGame()
    {
        // Implement turn order logic here with a state machine
    }

    private void HandleColumnClick(int column)
    {
        Disk diskPrefab = (currentPlayer == PlayerColor.Blue) ? blueDiskPrefab : redDiskPrefab;
        connectGameGrid.Spawn(diskPrefab, column, 0);
    }

    private void SetOpeningPlayer(PlayerColor playerColor)
    {
        BasePlayerController firstPlayer = playerControllers.FirstOrDefault(e => e.PlayerColor == playerColor);
        currentPlayer = firstPlayer.PlayerColor;
    }

    private void OnTurnEndedFunction(PlayerColor playerColor)
    {
        print(playerColor);
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
