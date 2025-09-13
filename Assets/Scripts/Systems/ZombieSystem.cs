using System.ComponentModel;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static GridDatabase;

public partial struct ZombieSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        ZombieMovmentJob movment = new ZombieMovmentJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        state.Dependency = movment.ScheduleParallel(state.Dependency);

        GridDatabasesSingleton gridDatabaseSingleton = SystemAPI.GetSingleton<GridDatabasesSingleton>();
        CachedGridDatabaseUnsafe unsafeGrid = new CachedGridDatabaseUnsafe()
        {
            entity = gridDatabaseSingleton.groundDatabase,
            gridDatabaseLookup = SystemAPI.GetComponentLookup<GridDatabase>(),
            gridCellLookup = SystemAPI.GetBufferLookup<GridCell>(false),
            gridCellElementLookup = SystemAPI.GetBufferLookup<GridCellElement>(false),
        };

        ZombieCollisionJob collision = new ZombieCollisionJob
        {
            cachedDatabase = unsafeGrid
        };

        state.Dependency = collision.ScheduleParallel(state.Dependency);

        ZombieApplyCollision applyCollision = new ZombieApplyCollision
        {
            
        };

        state.Dependency = applyCollision.Schedule(state.Dependency);

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public partial struct ZombieMovmentJob : IJobEntity
    {
        public float deltaTime;

        private void Execute(ref Unit unit, ref Health health, ref LocalTransform transform)
        {
            float3 targetVector = float3.zero - transform.Position;
            quaternion target = quaternion.LookRotationSafe(targetVector, math.up());
            transform.Rotation = target;
            float3 forward = math.mul(transform.Rotation, math.forward());
            transform.Position += forward * deltaTime;
        }

    }


    [BurstCompile]
    public partial struct ZombieCollisionJob : IJobEntity, IJobEntityChunkBeginEnd
    {
        public CachedGridDatabaseUnsafe cachedDatabase;
        private GridData _grid;


        private void Execute(Entity entity, ref Unit unit, ref LocalTransform lt, ref PhysicBody body, ref UnitBodyCollisionForce collisionForce, ref UnitCellID id)
        {
            GridBodyCollector collector = new GridBodyCollector { 
                position = lt.Position,
                bodyRadius = body.radius,
                bodyHeight = body.height,
                querier = entity,
                collisionForce = new float2(0)
            };
            GridDatabase.CellQueryAABB<GridBodyCollector>(in cachedDatabase.gridDatabase, in cachedDatabase.gridCellUnsafe, cachedDatabase.gridCellElementUnsafe, lt.Position, body.radius, ref collector);
            collisionForce.collisionForce = collector.collisionForce;
        }

        public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            cachedDatabase.CacheData();
            _grid = cachedDatabase.gridDatabase.gridData;
            return true;
        }

        public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
        {
        }
    }

    [BurstCompile]
    public partial struct ZombieApplyCollision : IJobEntity
    {
        private void Execute(ref Unit unit, ref LocalTransform lt, ref PhysicBody body, ref UnitBodyCollisionForce collisionForce)
        {
            float collision = math.lengthsq(collisionForce.collisionForce);
            if (collision > body.radius * body.radius)
                collisionForce.collisionForce = (collisionForce.collisionForce / math.sqrt(collision)) * body.radius;
            lt.Position += new float3(collisionForce.collisionForce.x, 0, collisionForce.collisionForce.y);
        }
    }

}
