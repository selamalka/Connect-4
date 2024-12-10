using System;
using UnityEngine;

public class AIPlayerController : BasePlayerController
{
    public override void MakeMove()
    {
        AIMove();
        TurnManager.OnTurnEnded?.Invoke(Id);
    }

    private void AIMove()
    {
        // Choose random valid columm
    }
}
