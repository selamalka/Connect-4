using MoonActive.Connect4;
using System;
using UnityEngine;

public abstract class BasePlayerController : MonoBehaviour
{
    [field: SerializeField] public PlayerColor PlayerColor;

    [HideInInspector]
    public GameManager GameManager;

    private void Awake()
    {
        GameManager = FindObjectOfType<GameManager>();
    }

    public abstract void MakeMove(int column);

    public bool IsMyTurn()
    {
        return PlayerColor == GameManager.CurrentPlayer;
    }
}