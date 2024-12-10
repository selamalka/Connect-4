using System;
using System.Collections;
using UnityEngine;

public class HumanPlayerController : BasePlayerController
{
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
                TurnManager.OnTurnEnded?.Invoke(Id);
                moveMade = true;
            }
            yield return null;
        }
    }
}

