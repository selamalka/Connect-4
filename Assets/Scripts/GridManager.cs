using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private int[,] gridState;
    [SerializeField] private int rows = 6;
    [SerializeField] private int columns = 7;

    private void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        gridState = new int[rows, columns];
    }

    public bool IsColumnAvailable(int column)
    {
        for (int row = 0; row < rows; row++)
        {
            if (gridState[row, column] == 0)
                return true;
        }
        return false;
    }

    public void UpdateGridState(int row, int column, int player)
    {
        gridState[row, column] = player;
    }

    public bool IsBoardFull()
    {
        for (int col = 0; col < columns; col++)
        {
            if (IsColumnAvailable(col))
                return false;
        }
        return true;
    }

    public int[,] GetGridState()
    {
        return gridState;
    }
}
