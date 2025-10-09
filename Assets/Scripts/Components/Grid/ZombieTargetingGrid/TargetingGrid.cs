using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TargetingGrid
{
    public int pointCountPerSide;
    public float halfSize;

    public float2 boundMin;
    public float2 boundMax;

    public TargetingGrid(float halfSize, int pointsPerSide)
    {
        this.halfSize = halfSize;
        this.pointCountPerSide = pointsPerSide;

        boundMin = new float2(-halfSize);
        boundMax = new float2(halfSize);
    }
}

[InternalBufferCapacity(0)]
public struct PathfindingPoint : IBufferElementData
{
    public float3 direction;
}

