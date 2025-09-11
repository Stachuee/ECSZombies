using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static GridDatabase;

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
            CachedSpatialDatabase = unsafeGrid
        };

        state.Dependency = assignUnitToCells.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    partial struct AssignUnitsToCells : IJobEntity, IJobEntityChunkBeginEnd
    {
        public CachedGridDatabaseUnsafe CachedSpatialDatabase;

        // other cached data
        private GridData _grid;


        public void Execute(in LocalToWorld ltw, ref UnitCellID sdCellIndex)
        {
            sdCellIndex.cellIndex = GridData.GetCellIndexFromPosition(in _grid, ltw.Position);
        }

        public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            CachedSpatialDatabase.CacheData();
            _grid = CachedSpatialDatabase.gridDatabase.gridData;
            return true;
        }

        public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
        {
        }
    }
}
