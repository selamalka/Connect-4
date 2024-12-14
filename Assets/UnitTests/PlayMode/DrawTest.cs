using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class DrawTest
{
    private GameObject gridManagerObject;
    private GridManager gridManager;

    private const int Rows = 6;
    private const int Columns = 7;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Create a new GridManager GameObject
        gridManagerObject = new GameObject("GridManager");
        gridManager = gridManagerObject.AddComponent<GridManager>();

        // Mock grid initialization
        InitializeMockGrid();

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        // Clean up after tests
        Object.DestroyImmediate(gridManagerObject);
        yield return null;
    }

    private void InitializeMockGrid()
    {
        // Access and initialize the gridCells array directly in GridManager
        var gridCellsField = typeof(GridManager).GetField("gridCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Cell[,] mockGridCells = new Cell[Rows, Columns];
        gridCellsField.SetValue(gridManager, mockGridCells);

        // Populate the mock gridCells array
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                var cellObject = new GameObject($"Cell[{row},{col}]");
                var cell = cellObject.AddComponent<Cell>();
                cell.SetRow(row);
                cell.SetColumn(col);
                cell.SetPlayerInCell(PlayerColor.None); // Start with empty cells
                mockGridCells[row, col] = cell;
            }
        }
    }

    [UnityTest]
    public IEnumerator CheckDraw_AllCellsFilled_ReturnsTrue()
    {
        // Fill the grid completely with alternating colors
        PlayerColor[] colors = { PlayerColor.Blue, PlayerColor.Red };

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                gridManager.GetCell(row, col).SetPlayerInCell(colors[(row + col) % 2]);
            }
        }

        // Assert that CheckDraw returns true
        Assert.IsTrue(gridManager.CheckDraw(), "CheckDraw did not detect a draw when the grid was full!");
        yield return null;
    }

    [UnityTest]
    public IEnumerator CheckDraw_EmptyCellsRemaining_ReturnsFalse()
    {
        // Fill the grid but leave one cell empty
        PlayerColor[] colors = { PlayerColor.Blue, PlayerColor.Red };

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                // Leave the last cell empty
                if (row == Rows - 1 && col == Columns - 1)
                {
                    gridManager.GetCell(row, col).SetPlayerInCell(PlayerColor.None);
                }
                else
                {
                    gridManager.GetCell(row, col).SetPlayerInCell(colors[(row + col) % 2]);
                }
            }
        }

        // Assert that CheckDraw returns false
        Assert.IsFalse(gridManager.CheckDraw(), "CheckDraw incorrectly detected a draw when empty cells were present!");
        yield return null;
    }
}
