using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    [Header("General")]
    public bool initializeOnApplicationStart;
    
    [Header("Prefabs")]
    [SerializeField]
    GameObject spatialGridDatabase;
    [SerializeField]
    GameObject pathfindingGridDatabase;
    [SerializeField]
    GameObject basicZombie;


    [Header("Spatial grid info")]
    public float halfSize;
    public int cellsPerSide;
    public int cellCapacity;

    [Header("Pathfinding grid info")]
    public float pathfindingHalfSize;
    public int pathfindingPointsPerSide;

    [Header("Physics cast info")]
    public int startingBufferSize;

    [Header("Debug")]
    public bool showSpatialGridSize;
    public bool showPathfindingGridSize;
    public bool spawnUnits;
    public int maxSpawnedUnits;



    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new Config
            {
                spatialGridInitialized = false,
                pathfindingGridInitialized = false,
                physicsCastInitialized = false,
                initializeOnApplicationStart = authoring.initializeOnApplicationStart,
                gridDatabasePrefab = GetEntity(authoring.spatialGridDatabase, TransformUsageFlags.None),
                pathfindingGridDatabasePrefab = GetEntity(authoring.pathfindingGridDatabase, TransformUsageFlags.None),
                basicZombie = GetEntity(authoring.basicZombie, TransformUsageFlags.Dynamic),
                halfSize = authoring.halfSize,
                cellCapacity = authoring.cellCapacity,
                cellsPerSide = authoring.cellsPerSide,
                pathfindingHalfSize = authoring.pathfindingHalfSize,
                pathfindingPointsPerSide = authoring.pathfindingPointsPerSide,
                spawnUnits = authoring.spawnUnits,
                maxSpawnedUnits = authoring.maxSpawnedUnits,
                startingBufferSize = authoring.startingBufferSize
            });
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showSpatialGridSize)
        {

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(default, new float3(halfSize * 2, 1, halfSize * 2));

            for (int i = 0; i < cellsPerSide; i++)
            {
                float z = -halfSize + (halfSize * 2 / cellsPerSide) * i;
                Gizmos.DrawLine(new float3(-halfSize, 0, z), new float3(halfSize, 0, z));
                Gizmos.DrawLine(new float3(z, 0, -halfSize), new float3(z, 0, halfSize));
            }
        }

        if(showPathfindingGridSize)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(default, new float3(pathfindingHalfSize * 2, 1, pathfindingHalfSize * 2));

            for (int i = 0; i <= pathfindingPointsPerSide; i++)
            {
                for (int j = 0; j <= pathfindingPointsPerSide; j++)
                {
                    float x = -pathfindingHalfSize + (pathfindingHalfSize * 2 / pathfindingPointsPerSide) * j;
                    float y = -pathfindingHalfSize + (pathfindingHalfSize * 2 / pathfindingPointsPerSide) * i;
                    Gizmos.DrawSphere(new Vector3(x, 0, y), 0.1f);
                }
            }
        }
  
    }

}
