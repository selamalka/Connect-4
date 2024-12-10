using MoonActive.Connect4;
using System;
using UnityEngine;

public abstract class BasePlayerController : MonoBehaviour
{
    [field: SerializeField] public PlayerColor PlayerColor;
    [field: SerializeField] public bool IsHuman { get; private set; }

    [HideInInspector]
    public ConnectGameGrid ConnectGameGrid;

    [HideInInspector]
    public GameManager GameManager;

    private void Awake()
    {
        ConnectGameGrid = FindObjectOfType<ConnectGameGrid>();
        GameManager = FindObjectOfType<GameManager>();
    }

    public abstract void MakeMove();
}