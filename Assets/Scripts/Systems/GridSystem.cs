using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static GridDatabase;

public interface IGridCollector
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnVisitCell(in GridCell cell, in UnsafeList<GridCellElement> elements, out bool exitEarly);
}

[UpdateInGroup(typeof(GridBuilding))]
public partial struct GridSystem : ISystem
{
    private EntityQuery databaseQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        databaseQuery = SystemAPI.QueryBuilder().WithAll<GridDatabase, GridCell, GridCellElement>().Build();

        state.RequireForUpdate<Config>();
        state.RequireForUpdate<GridDatabasesSingleton>();
        state.RequireForUpdate(databaseQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Config config = SystemAPI.GetSingleton<Config>();
        GridDatabasesSingleton gridDatabaseSingleton = SystemAPI.GetSingleton<GridDatabasesSingleton>();

        CachedGridDatabaseUnsafe unsafeGrid = new CachedGridDatabaseUnsafe()
        {
            entity = gridDatabaseSingleton.groundDatabase,
            gridDatabaseLookup = SystemAPI.GetComponentLookup<GridDatabase>(),
            gridCellLookup = SystemAPI.GetBufferLookup<GridCell>(false),
            gridCellElementLookup = SystemAPI.GetBufferLookup<GridCellElement>(false),
        };


        AssignUnitsToCells assignUnitToCells = new AssignUnitsToCells
        {
            cachedDatabase = unsafeGrid,
        };

        state.Dependency = assignUnitToCells.ScheduleParallel(state.Dependency);
        JobHandle initialDep = state.Dependency;
        int parallelCount = math.max(1, JobsUtility.JobWorkerCount - 1);

        for (int s = 0; s < parallelCount; s++)
        {
            BuildDatabase build = new BuildDatabase
            {
                JobSequenceNb = s,
                JobsTotalCount = parallelCount,
                cachedDatabase = unsafeGrid
            };
            state.Dependency = JobHandle.CombineDependencies(state.Dependency, build.Schedule(initialDep));
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    partial struct AssignUnitsToCells : IJobEntity, IJobEntityChunkBeginEnd
    {
        public CachedGridDatabaseUnsafe cachedDatabase;

        // other cached data
        private GridData _grid;


        public void Execute(in LocalToWorld ltw, ref UnitCellID sdCellIndex)
        {
            sdCellIndex.cellIndex = GridData.GetCellIndexFromPosition(in _grid, ltw.Position);
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
    partial struct BuildDatabase : IJobEntity, IJobEntityChunkBeginEnd
    {
        public int JobSequenceNb;
        public int JobsTotalCount;
        public CachedGridDatabaseUnsafe cachedDatabase;

        public void Execute(Entity entity, in LocalToWorld ltw, in UnitCellID cellId, in PhysicBody body, in Unit unit)
        {
            if(cellId.cellIndex % JobsTotalCount == JobSequenceNb)
            {
                GridCellElement element = new GridCellElement
                {
                    entity = entity,
                    postion = ltw.Position,
                    height = body.height,
                    radius = body.radius,
                    team = unit.team,
                };
                GridDatabase.AddToDatabase(in cachedDatabase.gridDatabase, ref cachedDatabase.gridCellUnsafe, ref cachedDatabase.gridCellElementUnsafe, element);
            }
        }

        public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            cachedDatabase.CacheData();
            return true;
        }

        public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
        {
        }
    }
}
