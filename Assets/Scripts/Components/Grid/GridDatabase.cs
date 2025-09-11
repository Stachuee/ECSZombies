using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct GridDatabase : IComponentData
{
    public GridData gridData;

    public static void CreataDatabase(float halfSize, int cellPerSide, int cellCapacity, ref GridDatabase gridDatabase, ref DynamicBuffer<GridCell> cells, ref DynamicBuffer<GridCellElement> elements)
    {
        gridDatabase.gridData = new GridData(halfSize, cellPerSide);

        //Set size of all buffers
        cells.Clear();
        elements.Clear();

        cells.Capacity = 16;
        elements.Capacity = 16;

        int cellCount = cellPerSide * cellPerSide;

        cells.Resize(cellCount, Unity.Collections.NativeArrayOptions.ClearMemory);
        elements.Resize(cellCount * cellCapacity, Unity.Collections.NativeArrayOptions.ClearMemory);

        // Clear and set all variables
        for(int i = 0; i < cellCount; i++)
        {
            GridCell cell = cells[i];
            cell.elementCapacity = cellCapacity;
            cell.startIndex = i * cellCapacity;
            cell.elementCount = 0;
            cell.excessElementCount = 0;
            cells[i] = cell;
        }
    }

    public static void ClearAndResize(ref DynamicBuffer<GridCell> cells, ref DynamicBuffer<GridCellElement> element)
    {
        int spaceAfterResize = 0;
        for(int i = 0; i < cells.Length; i++)
        {
            GridCell cell = cells[i];
            
            cell.startIndex = spaceAfterResize;
            cell.elementCapacity = math.select(cell.elementCapacity, (int)math.ceil((cell.elementCapacity + cell.excessElementCount) * 2), cell.excessElementCount > 0);
            spaceAfterResize += cell.elementCapacity;
            
            cell.elementCount = 0;
            cell.excessElementCount = 0;
            
            cells[i] = cell;
        }
        element.Resize(spaceAfterResize, Unity.Collections.NativeArrayOptions.ClearMemory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddToDatabase(in GridDatabase database, ref UnsafeList<GridCell> cells, ref UnsafeList<GridCellElement> elements, in GridCellElement toAdd)
    {
        int cellIndex = GridData.GetCellIndexFromPosition(in database.gridData, toAdd.postion);

        if(cellIndex >= 0)
        {
            GridCell cell = cells[cellIndex];

            if (cell.elementCount + 1 > cell.elementCapacity)
            {
                cell.excessElementCount++;
            }
            else
            {
                elements[cell.startIndex + cell.elementCount] = toAdd;
                cell.elementCount++;
            }

            cells[cellIndex] = cell;
        }
    }

    public unsafe struct CachedGridDatabaseUnsafe
    {
        public Entity entity;

        [NativeDisableParallelForRestriction]
        [NativeDisableContainerSafetyRestriction]
        public ComponentLookup<GridDatabase> gridDatabaseLookup;
        [NativeDisableParallelForRestriction]
        [NativeDisableContainerSafetyRestriction]
        public BufferLookup<GridCell> gridCellLookup;
        [NativeDisableParallelForRestriction]
        [NativeDisableContainerSafetyRestriction]
        public BufferLookup<GridCellElement> gridCellElementLookup;

        public bool ready;
        public GridDatabase gridDatabase;
        public UnsafeList<GridCell> gridCellUnsafe;
        public UnsafeList<GridCellElement> gridCellElementUnsafe;

        public void CacheData()
        {
            if(!ready)
            {
                gridDatabase = gridDatabaseLookup[entity];
                DynamicBuffer<GridCell> cellBuffer = gridCellLookup[entity];
                DynamicBuffer<GridCellElement> cellElementBuffer = gridCellElementLookup[entity];

                gridCellUnsafe = new UnsafeList<GridCell>((GridCell*)cellBuffer.GetUnsafePtr(), cellBuffer.Length);
                gridCellElementUnsafe = new UnsafeList<GridCellElement>((GridCellElement*)cellElementBuffer.GetUnsafePtr(), cellElementBuffer.Length);

                ready = true;
            }
        }

    }

    public unsafe static void CellQueryAABB<T>(in GridDatabase database,
    in DynamicBuffer<GridCell> cells, in DynamicBuffer<GridCellElement> elements, float3 center, float3 halfBoundSize,
    ref T collector) where T : unmanaged, IGridCollector
    {
        UnsafeList<GridCell> cellsUnsafe =
            new UnsafeList<GridCell>((GridCell*)cells.GetUnsafeReadOnlyPtr(), cells.Length);
        UnsafeList<GridCellElement> elementsUnsafe =
            new UnsafeList<GridCellElement>((GridCellElement*)elements.GetUnsafeReadOnlyPtr(), elements.Length);

        CellQueryAABB(in database, in cellsUnsafe, in elementsUnsafe, center, halfBoundSize, ref collector);
    }

    public static void CellQueryAABB<T>(in GridDatabase database,
        in UnsafeList<GridCell> cells, in UnsafeList<GridCellElement> elements, float3 center, float3 halfBoundSize,
        ref T collector) where T : unmanaged, IGridCollector
    {
        float3 aabbMin = center - halfBoundSize;
        float3 aabbMax = center + halfBoundSize;

        GridData grid = database.gridData;

        if(GridData.GetAABBMinMaxCoords(in grid, aabbMin, aabbMax, out int2 minCoords, out int2 maxCoords))
        {
            for(int y = minCoords.y; y <= maxCoords.y; y++)
            {
                for (int x = minCoords.x; x <= maxCoords.x; x++)
                {
                    int2 coords = new int2(x, y);
                    int cellIndex = GridData.GetCellIndexFromCoords(in grid, coords);
                    GridCell cell = cells[cellIndex];
                    collector.OnVisitCell(in cell, in elements, out bool exitEarly);

                    if(exitEarly)
                    {
                        return;
                    }
                }
            }
        }

    }

}
