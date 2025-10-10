using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct PathfindingGridSystem : ISystem
{
    private EntityQuery databaseQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        databaseQuery = SystemAPI.QueryBuilder().WithAll<PathfindingGridDatabase, PathfindingPoint>().Build();

        state.RequireForUpdate<Config>();
        state.RequireForUpdate<PathfindingGridSingleton>();
        state.RequireForUpdate(databaseQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Config config = SystemAPI.GetSingleton<Config>();
        PathfindingGridSingleton singleton = SystemAPI.GetSingleton<PathfindingGridSingleton>();

    }


}
