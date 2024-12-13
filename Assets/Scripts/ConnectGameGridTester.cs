using MoonActive.Connect4;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ConnectGameGridTester : MonoBehaviour, IGrid
{
    [SerializeField]
    [Tooltip("The grid creator component.")]
    private GridCreatorTester m_GridCreator;

    private List<DisksSpawnerTester> m_SpawnPoints;
    private List<Collider2D> m_Cells = new List<Collider2D>();

    public event Action<int> ColumnClicked;

    public IDisk Spawn(Disk diskPrefab, int column, int row)
    {
        FindAndEnableMatchingGridCollider(column, row);
        return m_SpawnPoints[column].CreateDiskAndResetPosition(diskPrefab);
    }

    public void SetGridCreator(GridCreatorTester gridCreator)
    {
        m_GridCreator = gridCreator;
    }

    private void Start()
    {
        m_GridCreator = FindObjectOfType<GridCreatorTester>();
        m_Cells = m_GridCreator.CreateGridCellColliders();
        m_SpawnPoints = m_GridCreator.CreateDiskSpawners();
        InitAllSpawners();
    }

    private void InitAllSpawners()
    {
        for (int i = 0; i < m_SpawnPoints.Count; i++)
        {
            int column = i;
            m_SpawnPoints[i].Init(delegate
            {
                this.ColumnClicked?.Invoke(column);
            });
        }
    }

    private void FindAndEnableMatchingGridCollider(int column, int row)
    {
        int index = 7 * row + column;
        m_Cells[index].enabled = true;
    }
}
