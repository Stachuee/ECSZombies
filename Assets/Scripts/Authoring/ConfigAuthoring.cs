using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    [Header("General")]
    public bool initializeOnApplicationStart;
    
    [Header("Prefabs")]
    [SerializeField]
    GameObject gridDatabase;

    [Header("Grid info")]
    public float halfSize;
    public int cellsPerSide;
    public int cellCapacity;

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
                halfSize = authoring.halfSize,
                cellCapacity = authoring.cellCapacity,
                cellsPerSide = authoring.cellsPerSide,
            });
        }
    }
}
