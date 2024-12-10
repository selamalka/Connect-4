using System;
using UnityEngine;

public class AIPlayerController : BasePlayerController
{
    public override void MakeMove(int column)
    {
        AIMove(column);
        GameManager.OnTurnEnded?.Invoke(PlayerColor);
    }

    private void AIMove(int column)
    {
        // Choose random valid columm
    }
}
