using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public struct PathfindingGrid
{
    public int pointCountPerSide;
    public float halfSize;
    public float pointsDistnace;

    public float2 boundMin;
    public float2 boundMax;

    public PathfindingGrid(float halfSize, int pointsPerSide)
    {
        this.halfSize = halfSize;
        this.pointCountPerSide = pointsPerSide;

        pointsDistnace = (halfSize * 2) / (pointCountPerSide - 1); // fill space from edge to edge

        boundMin = new float2(-halfSize);
        boundMax = new float2(halfSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 GetCoordsFromWorld(in PathfindingGrid grid, float2 position)
    {
        float2 local = position - grid.boundMin;
        int2 coords = new int2
        {
            x = (int)math.round(local.x / grid.pointsDistnace),
            y = (int)math.round(local.y / grid.pointsDistnace)
        };
        coords = math.clamp(coords, int2.zero, new int2(grid.pointCountPerSide - 1));
        return coords;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetCellIndexFromCoords(in PathfindingGrid grid, int2 coords)
    {
        return (coords.x) +
                (coords.y * grid.pointCountPerSide);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 GetCoordsFromIndex(in PathfindingGrid grid, int index)
    {
        return new int2
        {
            x = index % grid.pointCountPerSide,
            y = index / grid.pointCountPerSide
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float2 GetWorldFromCoords(in PathfindingGrid grid, int2 coords)
    {
        float2 pos = new float2
        {
            x = grid.boundMin.x + coords.x * grid.pointsDistnace,
            y = grid.boundMin.y + coords.y * grid.pointsDistnace
        };
        return pos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetClosestCorners(in PathfindingGrid grid, float2 position, 
        out int2 indexA, out int2 indexB, out int2 indexC, out int2 indexD)
    {
        float2 local = position - grid.boundMin;
        indexA = new int2
        {
            x = math.clamp((int)math.floor(local.x / grid.pointsDistnace), 0, grid.pointCountPerSide - 1),
            y = math.clamp((int)math.floor(local.y / grid.pointsDistnace), 0, grid.pointCountPerSide - 1)
        };
        indexB = new int2
        {
            x = math.clamp((int)math.floor(local.x / grid.pointsDistnace), 0, grid.pointCountPerSide - 1),
            y = math.clamp((int)math.ceil(local.y / grid.pointsDistnace), 0, grid.pointCountPerSide - 1)
        };
        indexC = new int2
        {
            x = math.clamp((int)math.ceil(local.x / grid.pointsDistnace), 0, grid.pointCountPerSide - 1),
            y = math.clamp((int)math.ceil(local.y / grid.pointsDistnace), 0, grid.pointCountPerSide - 1)
        };
        indexD = new int2
        {
            x = math.clamp((int)math.ceil(local.x / grid.pointsDistnace), 0, grid.pointCountPerSide - 1),
            y = math.clamp((int)math.floor(local.y / grid.pointsDistnace), 0, grid.pointCountPerSide - 1)
        };
    }
}

[InternalBufferCapacity(0)]
public struct PathfindingPoint : IBufferElementData
{
    public float2 walkDirection;
    public byte obscured;
}

