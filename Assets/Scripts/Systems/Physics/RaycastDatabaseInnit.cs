using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PhysicsInnitializing), OrderFirst = true)]
public partial struct RaycastDatabaseInnit : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out Config config) || SystemAPI.TryGetSingleton(out PhysicsCastDatabaseSingleton databases))
            return;

        if (config.physicsCastInitialized || config.initializeOnApplicationStart == false)
            return;

        Entity singleton = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponent<PhysicsCastDatabaseSingleton>(singleton);

        CreateDatabase(ref state, ref singleton, ref config);

        config.physicsCastInitialized = true;

        SystemAPI.SetSingleton<Config>(config);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    void CreateDatabase(ref SystemState state, ref Entity entity, ref Config config)
    {
        ref PhysicsCastDatabaseSingleton singleton = ref SystemAPI.GetSingletonRW<PhysicsCastDatabaseSingleton>().ValueRW;

        singleton.raycastDatabase =
            state.EntityManager.Instantiate(config.physicsCastDatabasePrefab);

        PhysicsCastDatabase database = state.EntityManager.GetComponentData<PhysicsCastDatabase>(singleton.raycastDatabase);
        DynamicBuffer<RaycastQuerry> raycastQuerry = state.EntityManager.GetBuffer<RaycastQuerry>(singleton.raycastDatabase);

        PhysicsCastDatabase.CreateDatabase(ref database, config.startingBufferSize, ref raycastQuerry);

        state.EntityManager.SetComponentData(singleton.raycastDatabase, database);
    }
}
