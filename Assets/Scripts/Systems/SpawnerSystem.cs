using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial struct SpawnerSystem : ISystem
{
    private EntityQuery units;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        units = SystemAPI.QueryBuilder().WithAll<Unit>().Build();
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Config config = SystemAPI.GetSingleton<Config>();

        if(config.spawnUnits && config.maxSpawnedUnits > units.CalculateEntityCount())
        {
            for(int i = 0; i < config.maxSpawnedUnits - units.CalculateEntityCount(); i++)
                state.EntityManager.Instantiate(config.basicZombie);
        }
    }
}
