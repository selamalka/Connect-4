
using UnityEngine;

public class HumanPlayerController : BasePlayerController
{
    public override void MakeMove()
    {
        // Wait for human input to select a column (handled via UI or mouse click).
        Debug.Log($"{PlayerColor} is waiting for input...");
    }
}
