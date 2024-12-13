using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;

public class WinTest
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

        // Add and mock the ConnectGameGridTester dependency
        var connectGameGridTester = gridManagerObject.AddComponent<ConnectGameGridTester>();
        SetupMockGridCreator(connectGameGridTester);

        // Initialize the mock grid
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

    private void SetupMockGridCreator(ConnectGameGridTester connectGameGridTester)
    {
        // Create a mock GridCreatorTester
        var gridCreatorObject = new GameObject("MockGridCreator");
        var mockGridCreator = gridCreatorObject.AddComponent<GridCreatorTester>();

        // Mock the colliders for grid cells
        var mockColliders = new List<Collider2D>();
        for (int i = 0; i < Rows * Columns; i++) // Assuming 6 rows * 7 columns = 42 cells
        {
            var cellObject = new GameObject($"CellCollider[{i}]");
            var collider = cellObject.AddComponent<BoxCollider2D>();
            mockColliders.Add(collider);
        }

        // Mock the spawners for disks
        var mockSpawners = new List<DisksSpawnerTester>();
        for (int i = 0; i < Columns; i++) // Assuming 7 columns
        {
            var spawnerObject = new GameObject($"DiskSpawner[{i}]");
            var spawner = spawnerObject.AddComponent<DisksSpawnerTester>();
            mockSpawners.Add(spawner);
        }

        // Assign the mock data to the mockGridCreator
        mockGridCreator.MockColliders = mockColliders;
        mockGridCreator.MockSpawners = mockSpawners;

        // Assign the mock GridCreatorTester to the ConnectGameGridTester
        connectGameGridTester.SetGridCreator(mockGridCreator);
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
                cell.SetPlayerInCell(PlayerColor.None);
                mockGridCells[row, col] = cell;
            }
        }
    }

    [UnityTest]
    public IEnumerator CheckWinHorizontal()
    {
        PlayerColor testPlayer = PlayerColor.Blue;

        for (int col = 0; col < 4; col++)
        {
            gridManager.GetCell(0, col).SetPlayerInCell(testPlayer);
        }

        Assert.IsTrue(gridManager.CheckWin(0, 3, testPlayer), "Horizontal win was not detected!");
        yield return null;
    }

    [UnityTest]
    public IEnumerator CheckWinVertical()
    {
        PlayerColor testPlayer = PlayerColor.Red;

        for (int row = 0; row < 4; row++)
        {
            gridManager.GetCell(row, 0).SetPlayerInCell(testPlayer);
        }

        Assert.IsTrue(gridManager.CheckWin(3, 0, testPlayer), "Vertical win was not detected!");
        yield return null;
    }

    [UnityTest]
    public IEnumerator CheckWinDiagonal()
    {
        PlayerColor testPlayer = PlayerColor.Blue;

        for (int i = 0; i < 4; i++)
        {
            gridManager.GetCell(i, i).SetPlayerInCell(testPlayer);
        }

        Assert.IsTrue(gridManager.CheckWin(3, 3, testPlayer), "Diagonal win was not detected!");
        yield return null;
    }

    [UnityTest]
    public IEnumerator CheckWinAntiDiagonal()
    {
        PlayerColor testPlayer = PlayerColor.Red;

        for (int i = 0; i < 4; i++)
        {
            var cell = gridManager.GetCell(i, 3 - i);
            Assert.IsNotNull(cell, $"Cell at ({i}, {3 - i}) is null.");
            cell.SetPlayerInCell(testPlayer);
        }

        Assert.IsTrue(gridManager.CheckWin(3, 0, testPlayer), "Anti-diagonal win was not detected!");
        yield return null;
    }

    [UnityTest]
    public IEnumerator NoWinDetected()
    {
        PlayerColor testPlayer = PlayerColor.Blue;
        PlayerColor otherPlayer = PlayerColor.Red;

        gridManager.GetCell(0, 0).SetPlayerInCell(testPlayer);
        gridManager.GetCell(0, 1).SetPlayerInCell(otherPlayer);
        gridManager.GetCell(0, 2).SetPlayerInCell(testPlayer);
        gridManager.GetCell(0, 3).SetPlayerInCell(otherPlayer);

        Assert.IsFalse(gridManager.CheckWin(0, 3, testPlayer), "False positive win detected!");
        yield return null;
    }
}
