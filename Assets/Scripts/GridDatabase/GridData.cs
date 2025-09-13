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
    public static int2 GetCoordsFromPostion(in GridData grid, float2 position)
    {
        float2 local = position - grid.boundMin;
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
    public static int GetCellIndexFromPosition(in GridData grid, float2 position)
    {
        if(CheckIfBounds(grid, position))
        {
            int2 cellCords = GetCoordsFromPostion(in grid, position);
            return GetCellIndexFromCoords(in grid, cellCords);
        }
        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetCellIndexFromPosition(in GridData grid, float3 position)
    {
        return GetCellIndexFromPosition(in grid, position.xz);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckIfBounds(in GridData grid, float2 position)
    {
        return (position.x > grid.boundMin.x || position.x < grid.boundMax.x ||
            position.y > grid.boundMin.y || position.y < grid.boundMax.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckIfBounds(in GridData grid, float3 position)
    {
        return CheckIfBounds(in grid, position.xz);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AABBIntersectAABB(float2 aabb1Min, float2 aabb1Max, float2 aabb2Min, float2 aabb2Max)
    {
        return (aabb1Min.x <= aabb2Max.x && aabb1Max.x >= aabb2Min.x) &&
               (aabb1Min.y <= aabb2Max.y && aabb1Max.y >= aabb2Min.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetAABBMinMaxCoords(in GridData grid, float2 aabbmin, float2 aabbmax, out int2 minCoords, out int2 maxCoords)
    {
        if(AABBIntersectAABB(aabbmin, aabbmax, grid.boundMin, grid.boundMax))
        {
            aabbmin = math.clamp(aabbmin, grid.boundMin, grid.boundMax);
            aabbmax = math.clamp(aabbmax, grid.boundMin, grid.boundMax);

            minCoords = GetCoordsFromPostion(in grid, aabbmin);
            maxCoords = GetCoordsFromPostion(in grid, aabbmax);

            return true;
        }
        minCoords = new int2(-1);
        maxCoords = new int2(-1);
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetAABBMinMaxCoords(in GridData grid, float3 aabbmin, float3 aabbmax, out int2 minCoords, out int2 maxCoords)
    {
        return GetAABBMinMaxCoords(in grid, aabbmin.xz, aabbmax.xz, out minCoords, out maxCoords);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetRealDistanceSq(float3 a, float3 b)
    {
        return GetRealDistanceSq(a.xz, b.xz);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetRealDistanceSq(float2 a, float2 b)
    {
        return math.distancesq(a, b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float2 GetDirection(float2 a, float2 b)
    {
        return b - a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float2 GetDirection(float3 a, float3 b)
    {
        return GetDirection(a.xz, b.xz);
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

    public float radius;
    public float height;
}