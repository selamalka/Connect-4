using MoonActive.Connect4;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private ConnectGameGrid connectGameGrid; // Reference to ConnectGameGrid
    [SerializeField] private int rows = 6;
    [SerializeField] private int columns = 7;

    private Cell[,] gridCells; // 2D array to manage Cell components

    private void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // Ensure gridCells matches the expected grid dimensions
        gridCells = new Cell[rows, columns];

        // Retrieve the array of cell colliders from ConnectGameGrid
        var cellColliders = connectGameGrid.GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < cellColliders.Length; i++)
        {
            Collider2D collider = cellColliders[i];
            Cell cell = collider.GetComponent<Cell>();

            if (cell != null)
            {
                // Calculate the row and column for this cell
                int row = i / columns;
                int column = i % columns;

                // Assign row, column, and initial state
                cell.SetRow(row);
                cell.SetColumn(column);
                cell.SetPlayerInCell(PlayerColor.None);
                cell.SetGridManager(this);

                collider.enabled = false;

                // Make sure only the bottom row's colliders will be enabled at the beginning
                if (cell.Row == 0) collider.enabled = true;

                // Add to the gridCells array for tracking
                gridCells[row, column] = cell;
            }
        }
    }

    /// <summary>
    /// Get the Cell at a specific grid position.
    /// </summary>
    public Cell GetCell(int row, int column)
    {
        if (row < 0 || row >= rows || column < 0 || column >= columns)
            return null;

        return gridCells[row, column];
    }

    public bool IsColumnFull(int column)
    {
        for (int i = 0; i < rows; i++)
        {
            if (GetCell(i, column).PlayerInCell == PlayerColor.None)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckWin(int row, int column, PlayerColor player)
    {
        // Directions to check: [row offset, column offset]
        int[,] directions = new int[,]
        {
        { 0, 1 },  // Horizontal (right-left)
        { 1, 0 },  // Vertical (up-down)
        { 1, 1 },  // Diagonal (\)
        { 1, -1 }  // Diagonal (/)
        };

        // Loop through each direction
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int rowDir = directions[i, 0];
            int colDir = directions[i, 1];

            int count = 1; // Include the current cell

            // Count in the positive direction
            count += CountInDirection(row, column, rowDir, colDir, player);

            // Count in the negative direction
            count += CountInDirection(row, column, -rowDir, -colDir, player);

            // Check if we have a win
            if (count >= 4)
            {
                Debug.Log($"Win detected for Player {player} starting at Row: {row}, Column: {column}");
                return true;
            }
        }

        return false; // No win detected
    }

    private int CountInDirection(int startRow, int startCol, int rowDir, int colDir, PlayerColor player)
    {
        int count = 0;

        int row = startRow + rowDir;
        int col = startCol + colDir;

        while (IsInBounds(row, col) && GetCell(row, col)?.PlayerInCell == player)
        {
            count++;
            row += rowDir;
            col += colDir;
        }

        return count;
    }

    private bool IsInBounds(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < columns;
    }

    public int GetNextAvailableRow(int column)
    {
        for (int row = 0; row < rows; row++)
        {
            if (GetCell(row, column).PlayerInCell == PlayerColor.None)
            {
                return row;
            }
        }

        Debug.LogError($"Column {column} is full but was not detected earlier!");
        return -1; // Column is full (should not happen if IsColumnFull was checked)
    }
}
