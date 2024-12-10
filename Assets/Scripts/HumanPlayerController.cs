
public class HumanPlayerController : BasePlayerController
{
    private void OnDisable()
    {
        GameManager.GetConnectGameGrid().ColumnClicked -= HandleColumnClicked;
    }

    private void Start()
    {
        GameManager.GetConnectGameGrid().ColumnClicked += HandleColumnClicked;
    }

    private void HandleColumnClicked(int column)
    {
        if (IsMyTurn())
        {
            MakeMove(column); 
        }
    }

    public override void MakeMove(int column)
    {
        print($"{PlayerColor} made a move");
        GameManager.OnTurnEnded?.Invoke(PlayerColor);
    }
}

