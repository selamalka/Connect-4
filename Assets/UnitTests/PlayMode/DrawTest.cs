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
        // Create a new GridManager GameObject and attach the GridManager component.
        gridManagerObject = new GameObject("GridManager");
        gridManager = gridManagerObject.AddComponent<GridManager>();

        // Initialize the mock grid for testing.
        InitializeMockGrid();

        yield return null; // Allow Unity to process setup operations.
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        // Destroy the GridManager GameObject after the test to clean up.
        Object.DestroyImmediate(gridManagerObject);
        yield return null; // Allow Unity to process cleanup operations.
    }

    private void InitializeMockGrid()
    {
        // Access and initialize the private gridCells array in GridManager.
        var gridCellsField = typeof(GridManager).GetField("gridCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Cell[,] mockGridCells = new Cell[Rows, Columns]; // Create a 2D array to hold mock cells.
        gridCellsField.SetValue(gridManager, mockGridCells);

        // Populate the grid with mock Cell objects.
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                var cellObject = new GameObject($"Cell[{row},{col}]"); // Name each cell uniquely.
                var cell = cellObject.AddComponent<Cell>(); // Add a Cell component to the GameObject.
                cell.SetRow(row); // Assign the row index to the cell.
                cell.SetColumn(col); // Assign the column index to the cell.
                cell.SetPlayerInCell(PlayerColor.None); // Initialize the cell as empty.
                mockGridCells[row, col] = cell; // Add the cell to the grid array.
            }
        }
    }

    [UnityTest]
    public IEnumerator CheckDrawAllCellsFilled()
    {
        // Fill the grid completely with alternating colors.
        PlayerColor[] colors = { PlayerColor.Blue, PlayerColor.Red }; // Define alternating colors.

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                // Alternate colors based on row and column indices.
                gridManager.GetCell(row, col).SetPlayerInCell(colors[(row + col) % 2]);
            }
        }

        // Assert that CheckDraw returns true when the grid is full.
        Assert.IsTrue(gridManager.CheckDraw(), "CheckDraw did not detect a draw when the grid was full!");

        yield return null; // Allow Unity to process frame updates.
    }

    [UnityTest]
    public IEnumerator CheckDrawEmptyCellsRemaining()
    {
        // Fill the grid but leave one cell empty.
        PlayerColor[] colors = { PlayerColor.Blue, PlayerColor.Red }; // Define alternating colors.

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                // Leave the last cell in the grid empty.
                if (row == Rows - 1 && col == Columns - 1)
                {
                    gridManager.GetCell(row, col).SetPlayerInCell(PlayerColor.None); // Leave cell empty.
                }
                else
                {
                    // Alternate colors based on row and column indices.
                    gridManager.GetCell(row, col).SetPlayerInCell(colors[(row + col) % 2]);
                }
            }
        }

        // Assert that CheckDraw returns false when the grid is not completely full.
        Assert.IsFalse(gridManager.CheckDraw(), "CheckDraw incorrectly detected a draw when empty cells were present!");

        yield return null; // Allow Unity to process frame updates.
    }
}
