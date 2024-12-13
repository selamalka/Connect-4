using System.Collections.Generic;
using UnityEngine;

public class GridCreatorTester : MonoBehaviour
{
    public List<Collider2D> MockColliders { get; set; } // Mocked grid cell colliders
    public List<DisksSpawnerTester> MockSpawners { get; set; } // Mocked disk spawners

    public List<Collider2D> CreateGridCellColliders()
    {
        // Return mocked colliders for testing
        return MockColliders ?? new List<Collider2D>();
    }

    public List<DisksSpawnerTester> CreateDiskSpawners()
    {
        // Return mocked spawners for testing
        return MockSpawners ?? new List<DisksSpawnerTester>();
    }
}