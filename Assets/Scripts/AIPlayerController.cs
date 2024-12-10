using System;
using UnityEngine;

public class AIPlayerController : BasePlayerController
{
    public override void MakeMove()
    {
        int column = UnityEngine.Random.Range(0, 6);
        //onMoveComplete?.Invoke(column);
    }
}
