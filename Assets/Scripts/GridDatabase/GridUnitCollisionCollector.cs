using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct GridUnitCollisionCollector : IGridCollector
{
    public float3 position;
    public float bodyRadius;
    public float bodyHeight;
    public bool staticBody;

    public Entity querier;
    public float2 collisionForce;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnVisitCell(in GridCell cell, in UnsafeList<GridCellElement> elements, out bool exitEarly)
    {
        exitEarly = false;

        for(int i = cell.startIndex; i < cell.startIndex + cell.elementCount; i++)
        {
            GridCellElement element = elements[i];

            if (staticBody || querier == element.entity)
                continue;

            float distSqr = GridData.GetRealDistanceSq(position, element.postion);
            float sumOfRadius = bodyRadius + element.radius;

            if (distSqr > sumOfRadius * sumOfRadius)
                continue;

            if (distSqr < 0.01 && querier.Index < element.entity.Index)
                position -= new float3(bodyRadius * 0.1f, 0, bodyRadius * 0.1f);

            float realDist = math.sqrt(distSqr);
            float2 normalized = GridData.GetDirection(position, element.postion) / realDist;
            float overlap = realDist - sumOfRadius;
            collisionForce += normalized * (overlap / 2);

        }

    }
}
