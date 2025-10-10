using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial struct PathfindingGridInnitSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out Config config) || SystemAPI.TryGetSingleton(out PathfindingGridSingleton databases))
            return;

        if (config.pathfindingGridInitialized || config.initializeOnApplicationStart == false)
            return;

        Entity singleton = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(singleton, new PathfindingGridSingleton());

        CreateDatabase(ref state, ref config, ref singleton);

        config.pathfindingGridInitialized = true;
        SystemAPI.SetSingleton(config);
    }

    [BurstCompile]
    void CreateDatabase(ref SystemState state, ref Config config, ref Entity entity)
    {
        ref PathfindingGridSingleton singleton = ref SystemAPI.GetSingletonRW<PathfindingGridSingleton>().ValueRW;

        singleton.targetingSystem =
            state.EntityManager.Instantiate(config.pathfindingGridDatabasePrefab);
        PathfindingGridDatabase database = state.EntityManager.GetComponentData<PathfindingGridDatabase>(singleton.targetingSystem);
        DynamicBuffer<PathfindingPoint> points = state.EntityManager.GetBuffer<PathfindingPoint>(singleton.targetingSystem);

        PathfindingGridDatabase.CreataDatabase(config.pathfindingHalfSize, config.pathfindingPointsPerSide, ref database, ref points);

        state.EntityManager.SetComponentData(singleton.targetingSystem, database);
    }
}
