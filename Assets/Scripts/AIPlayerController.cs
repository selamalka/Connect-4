using System;
using UnityEngine;

public class AIPlayerController : BasePlayerController
{
    public override void MakeMove()
    {
        AIMove();
        GameManager.OnTurnEnded?.Invoke(PlayerColor);
    }

    private void AIMove()
    {
        // Choose random valid columm
    }
}
