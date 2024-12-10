using UnityEngine;

public abstract class BasePlayerController : MonoBehaviour
{
    [field: SerializeField] public PlayerColor PlayerColor;

    public abstract void MakeMove(int column);
}