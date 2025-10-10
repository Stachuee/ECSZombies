using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

public struct PathfindingGridDatabase : IComponentData
{
    public TargetingGrid gridData;

    public static void CreataDatabase(float halfSize, int pointsPerSide,
        ref PathfindingGridDatabase gridDatabase, ref DynamicBuffer<PathfindingPoint> points)
    {
        gridDatabase.gridData = new TargetingGrid(halfSize, pointsPerSide);

        points.Clear();

        points.Capacity = 16;

        int pointCount = pointsPerSide * pointsPerSide;

        points.Resize(pointCount, NativeArrayOptions.ClearMemory);
    }

}
