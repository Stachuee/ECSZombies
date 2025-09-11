using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

public struct GridBodyCollector : IGridCollector
{
    float3 position;
    float bodyRadius;
    float bodyHeight;

    public float2 collisionForce;

    public GridBodyCollector(float3 position, float bodyRadius, float bodyHeight)
    {
        collisionForce = new float2(0);

        this.position = position;
        this.bodyRadius = bodyRadius;
        this.bodyHeight = bodyHeight;
    }

    public void OnVisitCell(in GridCell cell, in UnsafeList<GridCellElement> elements, out bool exitEarly)
    {
        exitEarly = false;
        collisionForce = new float2(0);


    }
}
