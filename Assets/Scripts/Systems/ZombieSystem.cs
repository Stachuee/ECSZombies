using System.ComponentModel;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static GridDatabase;
using static PathfindingGridDatabase;

[UpdateInGroup(typeof(UnitMovment))]
public partial struct ZombieSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Config config = SystemAPI.GetSingleton<Config>();

        ZombieInnit innit = new ZombieInnit
        {
            random = new Unity.Mathematics.Random((uint)(SystemAPI.Time.ElapsedTime * 1000) + 1)
        };

        state.Dependency = innit.Schedule(state.Dependency);

        PathfindingGridSingleton pathfindingDatabaseSingleton = SystemAPI.GetSingleton<PathfindingGridSingleton>();
        CachedPathfindingGridDatabaseUnsafe pathfindingDatabaseUnsafe = new CachedPathfindingGridDatabaseUnsafe()
        {
            entity = pathfindingDatabaseSingleton.targetingSystem,
            pathfindingDatabaseLookup = SystemAPI.GetComponentLookup<PathfindingGridDatabase>(),
            pathfindingPointsLookup = SystemAPI.GetBufferLookup<PathfindingPoint>()
        };

        ZombieMovmentJob movment = new ZombieMovmentJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            cachedPathfindingGridDatabase = pathfindingDatabaseUnsafe
            
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

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    [WithAll(typeof(Zombie))]
    public partial struct ZombieInnit : IJobEntity
    {
        public Unity.Mathematics.Random random;
        private void Execute(ref ZombieStats stats)
        {
            if (stats.innitialized)
                return;

            stats.speed += random.NextFloat(-stats.speed * 0.8f, stats.speed * 0.5f);
            stats.damage += random.NextFloat(-stats.damage * 0.8f, stats.damage * 0.5f);
            stats.innitialized = true;
        }
    }

    [BurstCompile]
    [WithAll(typeof(Zombie))]
    public partial struct ZombieMovmentJob : IJobEntity, IJobEntityChunkBeginEnd
    {
        public float deltaTime;
        public CachedPathfindingGridDatabaseUnsafe cachedPathfindingGridDatabase;

        private void Execute(ref Unit unit, ref Health health, ref LocalTransform transform, ref ZombiePathing pathing, ref ZombieStats stats)
        {
            float2 targetVector = PathfindingGridDatabase.GetWalkDirection(transform.Position, in cachedPathfindingGridDatabase.pathfindingGridDatabase, in cachedPathfindingGridDatabase.pathfindingPointsUnsafe);
            quaternion target = quaternion.LookRotationSafe(new float3(targetVector.x, 0, targetVector.y), math.up());
            transform.Rotation = target;
            float3 forward = math.mul(transform.Rotation, math.forward());
            transform.Position += forward * deltaTime * stats.speed;
        }

        public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            cachedPathfindingGridDatabase.CacheData();
            return true;
        }

        public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
        {
        }

    }



    [BurstCompile]
    [WithAll(typeof(Zombie))]
    public partial struct ZombieCollisionJob : IJobEntity, IJobEntityChunkBeginEnd
    {
        public CachedGridDatabaseUnsafe cachedDatabase;
        private GridData _grid;


        private void Execute(Entity entity, ref Unit unit, ref LocalTransform lt, ref PhysicBody body, ref UnitBodyCollisionForce collisionForce, ref UnitCellID id)
        {
            GridUnitCollisionCollector collector = new GridUnitCollisionCollector
            {
                position = lt.Position,
                bodyRadius = body.radius,
                bodyHeight = body.height,
                querier = entity,
                staticBody = body.staticBody,
                collisionForce = new float2(0)
            };
            GridDatabase.CellQueryAABB<GridUnitCollisionCollector>(in cachedDatabase.gridDatabase, in cachedDatabase.gridCellUnsafe, cachedDatabase.gridCellElementUnsafe, lt.Position, body.radius, ref collector);
            collisionForce.collisionForce = collector.collisionForce;

            float collision = math.lengthsq(collisionForce.collisionForce);
            if (collision > body.radius * body.radius)
                collisionForce.collisionForce = (collisionForce.collisionForce / math.sqrt(collision)) * body.radius;
            lt.Position += new float3(collisionForce.collisionForce.x, 0, collisionForce.collisionForce.y);
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

}
