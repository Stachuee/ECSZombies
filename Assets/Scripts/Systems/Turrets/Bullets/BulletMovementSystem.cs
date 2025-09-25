using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static GridDatabase;

public partial struct BulletMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        GridDatabasesSingleton gridDatabaseSingleton = SystemAPI.GetSingleton<GridDatabasesSingleton>();

        CachedGridDatabaseUnsafe unsafeGrid = new CachedGridDatabaseUnsafe()
        {
            entity = gridDatabaseSingleton.groundDatabase,
            gridDatabaseLookup = SystemAPI.GetComponentLookup<GridDatabase>(),
            gridCellLookup = SystemAPI.GetBufferLookup<GridCell>(false),
            gridCellElementLookup = SystemAPI.GetBufferLookup<GridCellElement>(false),
        };

        BulletJob bulletJob = new BulletJob()
        {
            unsafeGrid = unsafeGrid,
            deltaTime = SystemAPI.Time.DeltaTime
        };

        state.Dependency = bulletJob.ScheduleParallel(state.Dependency);


        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        DisposeBullets disposeBullets = new DisposeBullets()
        {
            //ECB = ecb
            ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        };

        state.Dependency = disposeBullets.Schedule(state.Dependency);
        state.Dependency.Complete();

        //ecb.Playback(state.EntityManager);
        //ecb.Dispose();
    }

    [BurstCompile]
    partial struct BulletJob : IJobEntity, IJobEntityChunkBeginEnd
    {
        public CachedGridDatabaseUnsafe unsafeGrid;
        public float deltaTime;

        public void Execute(ref LocalTransform lt, ref Bullet bullet)
        {
            float3 prevoiusPosition = lt.Position;
            lt.Position += bullet.direction * bullet.speed * deltaTime;

            LinecastCollector collector = new LinecastCollector()
            {
                start = prevoiusPosition,
                end = lt.Position,
            };

            GridDatabase.CallQueryLineCast<LinecastCollector>(in unsafeGrid.gridDatabase, in unsafeGrid.gridCellUnsafe, in unsafeGrid.gridCellElementUnsafe,
                in prevoiusPosition, in lt.Position, ref collector);
            bullet.dispose = collector.hit;
        }
        public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            unsafeGrid.CacheData();
            return true;
        }

        public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
        {

        }
    }

    [BurstCompile]
    partial struct DisposeBullets : IJobEntity, IJobEntityChunkBeginEnd
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        private int _chunkIndex;

        public void Execute(Entity entity, in Bullet bullet)
        {
            if (!bullet.dispose)
                return;
            ECB.DestroyEntity(_chunkIndex, entity);
        }

        public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            _chunkIndex = unfilteredChunkIndex;
            return true;
        }

        public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask,
            bool chunkWasExecuted)
        { }
    }

}
