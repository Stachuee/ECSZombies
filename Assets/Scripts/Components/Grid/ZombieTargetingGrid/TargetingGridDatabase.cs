using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct TargetingGridDatabase : IComponentData
{
    public TargetingGrid gridData;

    public static void CreataDatabase(float halfSize, int pointsPerSide,
        ref TargetingGridDatabase gridDatabase, ref DynamicBuffer<PathfindingPoint> points)
    {
        gridDatabase.gridData = new TargetingGrid(halfSize, pointsPerSide);

        points.Clear();

        points.Capacity = 16;

        int pointCount = pointsPerSide * pointsPerSide;

        points.Resize(pointCount, NativeArrayOptions.ClearMemory);
    }
}
