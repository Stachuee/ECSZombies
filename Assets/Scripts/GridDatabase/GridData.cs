using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct GridData
{
    public int cellCountPerSide;
    public float halfSize;

    public int cellCount;
    public float cellSize;

    public float2 boundMin;
    public float2 boundMax;

    public GridData(float halfSize, int cellsPerSide)
    {
        this.cellCountPerSide = cellsPerSide;
        this.halfSize = halfSize;

        cellCount = cellsPerSide * cellsPerSide;
        cellSize = (halfSize * 2) / cellsPerSide;

        boundMin = new float2(-halfSize);
        boundMax = new float2(halfSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 GetCoordsFromPostion(in GridData grid, float3 position)
    {
        float2 local = position.xz - grid.boundMin;
        int2 coords = new int2
        {
            x = (int)math.floor(local.x / grid.cellSize),
            y = (int)math.floor(local.y / grid.cellSize)
        };
        coords = math.clamp(coords, int2.zero, new int2(grid.cellCountPerSide - 1));
        return coords;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 GetCoordsFromIndex(in GridData grid, int index)
    {
        return new int2
        {
            x = index % grid.cellCountPerSide,
            y = index / grid.cellCountPerSide
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetCellIndexFromCoords(in GridData grid, int2 coords)
    {
        return (coords.x) +
                (coords.y * grid.cellCountPerSide);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetCellIndexFromPosition(in GridData grid, float3 position)
    {
        if(CheckIfBounds(grid, position))
        {
            int2 cellCords = GetCoordsFromPostion(grid, position);
            return GetCellIndexFromCoords(grid, cellCords);
        }
        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckIfBounds(in GridData grid, float3 position)
    {
        return (position.x > grid.boundMin.x || position.x < grid.boundMax.x ||
            position.y > grid.boundMin.y || position.y < grid.boundMax.y);
    }
}

[InternalBufferCapacity(0)]
public struct GridCell : IBufferElementData
{
    public int startIndex;
    public int elementCount;
    public int elementCapacity;
    public int excessElementCount;
}

[InternalBufferCapacity(0)]
public struct GridCellElement : IBufferElementData
{
    public Entity entity;
    public float3 postion;
    public byte type;
}