using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    [Header("General")]
    public bool initializeOnApplicationStart;
    
    [Header("Prefabs")]
    [SerializeField]
    GameObject gridDatabase;
    [SerializeField]
    GameObject basicZombie;

    [Header("Grid info")]
    public float halfSize;
    public int cellsPerSide;
    public int cellCapacity;

    [Header("Debug")]
    public bool spawnUnits;
    public int maxSpawnedUnits;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new Config
            {
                initialized = false,
                initializeOnApplicationStart = authoring.initializeOnApplicationStart,
                gridDatabasePrefab = GetEntity(authoring.gridDatabase, TransformUsageFlags.None),
                basicZombie = GetEntity(authoring.basicZombie, TransformUsageFlags.Dynamic),
                halfSize = authoring.halfSize,
                cellCapacity = authoring.cellCapacity,
                cellsPerSide = authoring.cellsPerSide,
                spawnUnits = authoring.spawnUnits,
                maxSpawnedUnits = authoring.maxSpawnedUnits,

            });
        }
    }

    private void OnDrawGizmosSelected()
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
}
