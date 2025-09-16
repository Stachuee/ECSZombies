using System.Diagnostics.Tracing;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[UpdateInGroup(typeof(GridInitializing))]
public partial struct GridInnit : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out Config config) || SystemAPI.TryGetSingleton(out GridDatabasesSingleton databases))
            return;

        if (config.gridInitialized || config.initializeOnApplicationStart == false)
            return;

        Entity singleton = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(singleton, new GridDatabasesSingleton());

        CreateDatabase(ref state, ref config, ref singleton);

        config.gridInitialized = true;
        SystemAPI.SetSingleton(config);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    void CreateDatabase(ref SystemState state, ref Config config, ref Entity entity)
    {
        ref GridDatabasesSingleton gridDatabaseSingleton = ref SystemAPI.GetSingletonRW<GridDatabasesSingleton>().ValueRW;

        gridDatabaseSingleton.groundDatabase =
            state.EntityManager.Instantiate(config.gridDatabasePrefab);
        GridDatabase database = state.EntityManager.GetComponentData<GridDatabase>(gridDatabaseSingleton.groundDatabase);
        DynamicBuffer<GridCell> cells = state.EntityManager.GetBuffer<GridCell>(gridDatabaseSingleton.groundDatabase);
        DynamicBuffer<GridCellElement> elements = state.EntityManager.GetBuffer<GridCellElement>(gridDatabaseSingleton.groundDatabase);

        GridDatabase.CreataDatabase(config.halfSize, config.cellsPerSide, config.cellCapacity, ref database, ref cells, ref elements);

        state.EntityManager.SetComponentData(gridDatabaseSingleton.groundDatabase, database);
    }

}
