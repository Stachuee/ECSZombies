using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static GridDatabase;

[UpdateInGroup(typeof(BuildingActions), OrderFirst = true)]
public partial struct TargetingSystem : ISystem
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

        TargetingSystemJob targetingJob = new TargetingSystemJob()
        {
            unsafeGrid = unsafeGrid,
            time = (float) SystemAPI.Time.ElapsedTime
        };

        state.Dependency = targetingJob.Schedule(state.Dependency);

    }


    public partial struct TargetingSystemJob : IJobEntity, IJobEntityChunkBeginEnd
    {
        public CachedGridDatabaseUnsafe unsafeGrid;
        public float time;

        public void Execute(Entity entity, ref Targeting targeting, ref LocalTransform lt, ref Unit unit)
        {
            if (targeting.nextUpdate > time)
                return;

            targeting.nextUpdate = time + targeting.timeBetweenUpdates;

            GridTargetCollector collector = new GridTargetCollector
            {
                targetingType = targeting.targetingType,
                targetingTeamType = targeting.targetingTeamType,
                targetingRange = targeting.targetingRange,
                position = lt.Position,
                team = unit.team
            };

            GridDatabase.CellQueryAABB<GridTargetCollector>(in unsafeGrid.gridDatabase, in unsafeGrid.gridCellUnsafe, in unsafeGrid.gridCellElementUnsafe,
                lt.Position, targeting.targetingRange, ref collector);

            targeting.targetAvalible = collector.foundAvalibleTarget;
            targeting.targetPosition = collector.targetPosition;
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
}
