using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct SpawnerSystem : ISystem
{
    private EntityQuery units;
    public Unity.Mathematics.Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        units = SystemAPI.QueryBuilder().WithAll<Unit>().Build();
        state.RequireForUpdate<Config>();
        random = new Unity.Mathematics.Random(999);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Config config = SystemAPI.GetSingleton<Config>();

        if(config.spawnUnits && config.maxSpawnedUnits > units.CalculateEntityCount())
        {
            for(int i = 0; i < config.maxSpawnedUnits - units.CalculateEntityCount(); i++)
            {
                Entity entity = state.EntityManager.Instantiate(config.basicZombie);
                double x = random.NextFloat(0, 1f) * config.halfSize * 2 - config.halfSize;
                double y = random.NextFloat(0, 1f) * config.halfSize * 2 - config.halfSize;

                state.EntityManager.SetComponentData(entity, new LocalTransform
                {
                    Position = new float3((float)x, 0, (float)y),
                    Scale = 1
                });
            }
        }
    }
}
