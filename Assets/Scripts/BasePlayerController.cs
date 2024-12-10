using System;
using UnityEngine;

public abstract class BasePlayerController : MonoBehaviour
{
    [field: SerializeField] public PlayerColor Color;
    [field :SerializeField] public int Id {  get; private set; }

    public abstract void MakeMove();
}
