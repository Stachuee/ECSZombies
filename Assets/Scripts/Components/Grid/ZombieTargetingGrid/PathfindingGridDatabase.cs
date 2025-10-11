using System.Drawing;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PathfindingGridDatabase : IComponentData
{
    public PathfindingGrid gridData;

    public static void CreataDatabase(float halfSize, int pointsPerSide,
        ref PathfindingGridDatabase gridDatabase, ref DynamicBuffer<PathfindingPoint> points)
    {
        gridDatabase.gridData = new PathfindingGrid(halfSize, pointsPerSide);

        points.Clear();

        points.Capacity = 16;

        int pointCount = pointsPerSide * pointsPerSide;

        points.Resize(pointCount, NativeArrayOptions.ClearMemory);
        GenerateDefaultPathfindingGrid(ref gridDatabase, ref points);
    }

    public static void AddObstacle(ref PathfindingGridDatabase database, ref NativeArray<float2> borders, ref DynamicBuffer<PathfindingPoint> points)
    {
        if (points.Length <= 2)
            throw new System.Exception("Uncompleated obsticle");

        for(int y = 0; y < database.gridData.pointCountPerSide; y++)
        {
            for (int x = 0; x < database.gridData.pointCountPerSide; x++)
            {
                bool inside = MathFunctions.CheckIfPointInsidePolygon(ref borders, PathfindingGrid.GetWorldFromCoords(database.gridData, new int2(x, y)));
                if (!inside)
                    continue;

                int index = PathfindingGrid.GetCellIndexFromCoords(database.gridData, new int2(x, y));
                PathfindingPoint point = points[index];
                point.obscured += 1;
                points[index] = point;

            }
        }
    }

    public static void GenerateDefaultPathfindingGrid(ref PathfindingGridDatabase database, ref DynamicBuffer<PathfindingPoint> points)
    {
        for (int y = 0; y < database.gridData.pointCountPerSide; y++)
        {
            for (int x = 0; x < database.gridData.pointCountPerSide; x++)
            {
                float2 directionVector = new float2(0, 0) - PathfindingGrid.GetWorldFromCoords(database.gridData, new int2(x, y));
                int index = PathfindingGrid.GetCellIndexFromCoords(database.gridData, new int2(x, y));
                PathfindingPoint point = points[index];
                point.walkDirection = math.normalizesafe(directionVector);
                point.obscured = 0;
                points[index] = point;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float2 GetWalkDirection(float2 position, in PathfindingGridDatabase database, in UnsafeList<PathfindingPoint> points)
    {
        int2 A, B, C, D;
        PathfindingGrid.GetClosestCorners(in database.gridData, position, out A, out B, out C, out D);
        float2 min = PathfindingGrid.GetWorldFromCoords(in database.gridData, A);
        float2 max = PathfindingGrid.GetWorldFromCoords(in database.gridData, C);

        float vertical = (max.y - min.y) / database.gridData.pointsDistnace;
        float horizontal = (max.x - min.x) / database.gridData.pointsDistnace;

        return (1f - vertical) * (1f - horizontal) * points[PathfindingGrid.GetCellIndexFromCoords(database.gridData, A)].walkDirection +
            (1f - vertical) * horizontal * points[PathfindingGrid.GetCellIndexFromCoords(database.gridData, B)].walkDirection +
            vertical * horizontal * points[PathfindingGrid.GetCellIndexFromCoords(database.gridData, C)].walkDirection +
            vertical * (1f - horizontal) * points[PathfindingGrid.GetCellIndexFromCoords(database.gridData, D)].walkDirection;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float2 GetWalkDirection(float3 position, in PathfindingGridDatabase database, in UnsafeList<PathfindingPoint> points)
    {
        return GetWalkDirection(new float2(position.x, position.z), in database, in points);
    }


    public static void UpdateGrid()
    {

    }
    public unsafe struct CachedPathfindingGridDatabaseUnsafe
    {
        public Entity entity;

        [NativeDisableParallelForRestriction]
        [NativeDisableContainerSafetyRestriction]
        public ComponentLookup<PathfindingGridDatabase> pathfindingDatabaseLookup;
        [NativeDisableParallelForRestriction]
        [NativeDisableContainerSafetyRestriction]
        public BufferLookup<PathfindingPoint> pathfindingPointsLookup;

        public bool ready;
        public PathfindingGridDatabase pathfindingGridDatabase;
        public UnsafeList<PathfindingPoint> pathfindingPointsUnsafe;

        public void CacheData()
        {
            if (!ready)
            {
                pathfindingGridDatabase = pathfindingDatabaseLookup[entity];
                DynamicBuffer<PathfindingPoint> cellBuffer = pathfindingPointsLookup[entity];

                pathfindingPointsUnsafe = new UnsafeList<PathfindingPoint>((PathfindingPoint*)cellBuffer.GetUnsafePtr(), cellBuffer.Length);

                ready = true;
            }
        }

    }
}
