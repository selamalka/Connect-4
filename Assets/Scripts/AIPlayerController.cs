using System;
using UnityEngine;

public class AIPlayerController : BasePlayerController
{
    [SerializeField] private int gridWidth;

    public override void MakeMove()
    {
        int column = UnityEngine.Random.Range(0, gridWidth);
        //onMoveComplete?.Invoke(column);
    }
}
