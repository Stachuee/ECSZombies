using Unity.Entities;
using UnityEngine;

public struct Config : IComponentData
{
    [Header("General")]
    public bool initialized;
    public bool initializeOnApplicationStart;
    [Header("Prefabs")]
    public Entity gridDatabasePrefab;

    [Header("Grid info")]
    public float halfSize;
    public int cellsPerSide;
    public int cellCapacity;
}
