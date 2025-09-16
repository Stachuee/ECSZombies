using Unity.Entities;
using UnityEngine;

public struct Config : IComponentData
{
    [Header("General")]
    public bool gridInitialized;
    public bool physicsCastInitialized;
    public bool initializeOnApplicationStart;
    [Header("Prefabs")]
    public Entity gridDatabasePrefab;
    public Entity physicsCastDatabasePrefab;
    public Entity basicZombie;

    [Header("Grid info")]
    public float halfSize;
    public int cellsPerSide;
    public int cellCapacity;

    [Header("Physics cast info")]
    public int startingBufferSize;

    [Header("Debug")]
    public bool spawnUnits;
    public float maxSpawnedUnits;
}
