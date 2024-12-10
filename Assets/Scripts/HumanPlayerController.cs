using MoonActive.Connect4;
using System;
using System.Collections;
using UnityEngine;

public class HumanPlayerController : BasePlayerController
{    
    private void OnDisable()
    {
        GameManager.GetConnectGameGrid().ColumnClicked -= HandleColumnClicked;
    }

    private void Start()
    {
        GameManager.GetConnectGameGrid().ColumnClicked += HandleColumnClicked;
    }

    public override void MakeMove()
    {
        StartCoroutine(WaitForPlayerInput());
    }

    private IEnumerator WaitForPlayerInput()
    {
        bool moveMade = false;

        while (!moveMade)
        {
            if (Input.GetMouseButtonDown(0))
            {
                print("move was made");
                GameManager.OnTurnEnded?.Invoke(PlayerColor);
                moveMade = true;
            }
            yield return null;
        }
    }

    private void HandleColumnClicked(int column)
    {
        Disk diskPrefab = GameManager.GetDiskByPlayerColor(PlayerColor);
        ConnectGameGrid.Spawn(diskPrefab, column, 0);
    }
}

