using Unity.Entities;
using UnityEngine;

public struct Config : IComponentData
{
    [Header("General")]
    public bool spatialGridInitialized;
    public bool pathfindingGridInitialized;
    public bool physicsCastInitialized;
    public bool initializeOnApplicationStart;

    [Header("Prefabs")]
    public Entity gridDatabasePrefab;
    public Entity pathfindingGridDatabasePrefab;
    public Entity basicZombie;

    [Header("Grid info")]
    public float halfSize;
    public int cellsPerSide;
    public int cellCapacity;

    [Header("Pathfinding grid info")]
    public float pathfindingHalfSize;
    public int pathfindingPointsPerSide;

    [Header("Physics cast info")]
    public int startingBufferSize;

    [Header("Debug")]
    public bool spawnUnits;
    public float maxSpawnedUnits;
}
