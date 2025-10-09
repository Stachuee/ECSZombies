using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial struct PathfindingGridInnitSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out Config config) || SystemAPI.TryGetSingleton(out TargetingGridSingleton databases))
            return;

        if (config.pathfindingGridInitialized || config.initializeOnApplicationStart == false)
            return;

        Entity singleton = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(singleton, new TargetingGridSingleton());

        CreateDatabase(ref state, ref config, ref singleton);

        config.pathfindingGridInitialized = true;
        SystemAPI.SetSingleton(config);
    }

    //[BurstCompile]
    void CreateDatabase(ref SystemState state, ref Config config, ref Entity entity)
    {
        ref TargetingGridSingleton singleton = ref SystemAPI.GetSingletonRW<TargetingGridSingleton>().ValueRW;

        singleton.targetingSystem =
            state.EntityManager.Instantiate(config.pathfindingGridDatabasePrefab);
        TargetingGridDatabase database = state.EntityManager.GetComponentData<TargetingGridDatabase>(singleton.targetingSystem);
        DynamicBuffer<PathfindingPoint> points = state.EntityManager.GetBuffer<PathfindingPoint>(singleton.targetingSystem);

        TargetingGridDatabase.CreataDatabase(config.pathfindingHalfSize, config.pathfindingPointsPerSide, ref database, ref points);

        state.EntityManager.SetComponentData(singleton.targetingSystem, database);
    }
}
