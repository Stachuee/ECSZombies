using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(GridBuilding), OrderFirst = true)]
public partial struct GridClearSystem : ISystem
{
    private EntityQuery databaseQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        databaseQuery = SystemAPI.QueryBuilder().WithAll<GridDatabase, GridCell, GridCellElement>().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if(databaseQuery.CalculateEntityCount() > 0)
        {
            BufferLookup<GridCell> cellsLookup = SystemAPI.GetBufferLookup<GridCell>(false);
            BufferLookup<GridCellElement> cellElementsLookup = SystemAPI.GetBufferLookup<GridCellElement>(false);
            NativeArray<Entity> databases = databaseQuery.ToEntityArray(Allocator.Temp);
            
            JobHandle initialDep = state.Dependency;

            for (int i = 0; i < databases.Length; i++)
            {
                ClearGrid clearJob = new ClearGrid
                {
                    Entity = databases[i],
                    CellsBufferLookup = cellsLookup,
                    ElementsBufferLookup = cellElementsLookup,
                };
                state.Dependency = JobHandle.CombineDependencies(state.Dependency, clearJob.Schedule(initialDep));
            }

            databases.Dispose();
        }
    }

    [BurstCompile]
    public partial struct ClearGrid : IJobEntity
    {
        public Entity Entity;
        public BufferLookup<GridCell> CellsBufferLookup;
        public BufferLookup<GridCellElement> ElementsBufferLookup;

        public void Execute()
        {
            if (CellsBufferLookup.TryGetBuffer(Entity, out DynamicBuffer<GridCell> cellsBuffer) &&
                ElementsBufferLookup.TryGetBuffer(Entity, out DynamicBuffer<GridCellElement> elementsBuffer))
            {
                GridDatabase.ClearAndResize(ref cellsBuffer, ref elementsBuffer);
            }
        }
    }

}