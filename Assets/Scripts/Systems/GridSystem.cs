using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial struct GridSystem : ISystem
{
    private EntityQuery databaseQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        databaseQuery = SystemAPI.QueryBuilder().WithAll<GridDatabase>().Build();

        state.RequireForUpdate(databaseQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }


}
