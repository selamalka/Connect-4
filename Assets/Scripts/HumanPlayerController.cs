
public class HumanPlayerController : BasePlayerController
{
    public override void MakeMove(int column)
    {
        GameManager.OnTurnEnded?.Invoke(PlayerColor);
    }
}

