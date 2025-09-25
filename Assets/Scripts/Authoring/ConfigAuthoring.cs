using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    [Header("General")]
    public bool initializeOnApplicationStart;
    
    [Header("Prefabs")]
    [SerializeField]
    GameObject gridDatabase;
    [SerializeField]
    GameObject physicsCastDatabasePrefab;
    [SerializeField]
    GameObject basicZombie;


    [Header("Grid info")]
    public float halfSize;
    public int cellsPerSide;
    public int cellCapacity;

    [Header("Physics cast info")]
    public int startingBufferSize;

    [Header("Debug")]
    public bool showGridSize;
    public bool spawnUnits;
    public int maxSpawnedUnits;

    [Header("PhysicsCastTest")]
    public bool lineCastTest;
    public bool lineCastTestTwo;
    public float2 start;
    public float2 end;
    public float2 circlePos;
    public float circleRadius;


    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new Config
            {
                gridInitialized = false,
                physicsCastInitialized = false,
                initializeOnApplicationStart = authoring.initializeOnApplicationStart,
                gridDatabasePrefab = GetEntity(authoring.gridDatabase, TransformUsageFlags.None),
                basicZombie = GetEntity(authoring.basicZombie, TransformUsageFlags.Dynamic),
                physicsCastDatabasePrefab = GetEntity(authoring.physicsCastDatabasePrefab, TransformUsageFlags.None),
                halfSize = authoring.halfSize,
                cellCapacity = authoring.cellCapacity,
                cellsPerSide = authoring.cellsPerSide,
                spawnUnits = authoring.spawnUnits,
                maxSpawnedUnits = authoring.maxSpawnedUnits,
                startingBufferSize = authoring.startingBufferSize
            });
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showGridSize)
        {

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(default, new float3(halfSize * 2, 1, halfSize * 2));

            for (int i = 0; i < cellsPerSide; i++)
            {
                float z = -halfSize + (halfSize * 2 / cellsPerSide) * i;
                Gizmos.DrawLine(new float3(-halfSize, 0, z), new float3(halfSize, 0, z));
                Gizmos.DrawLine(new float3(z, 0, -halfSize), new float3(z, 0, halfSize));
            }
        }

        if(lineCastTest)
        {

            GridData data = new GridData(halfSize, cellsPerSide);



            float a = (end.y - start.y) / (end.x - start.x);
            float b = start.y - a * start.x;

            float2 dir = end - start;
            float2 current = new float2(start.x, start.y);


            float signX = dir.x > 0 ? data.cellSize * 0.5f : data.cellSize * -0.5f;
            float signY = dir.y > 0 ? data.cellSize * 0.5f : data.cellSize * -0.5f;

            int2 currentIndex = GridData.GetCoordsFromPostion(in data, current);

            int2 endIndex = GridData.GetCoordsFromPostion(in data, end);

            float2 X;
            float2 Y;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(start.x, 2, start.y), new Vector3(end.x, 2, end.y));
            

            for (int i = 0; i < 20; i++)
            {

                Gizmos.color = Color.red;
                float2 pos = GridData.GetWorldPositionOfAMiddleOfACell(in data, currentIndex);
                Gizmos.DrawCube(new Vector3(pos.x, 0, pos.y), data.cellSize * Vector3.one * 0.2f);

                if (currentIndex.x == endIndex.x && currentIndex.y == endIndex.y)
                    break;


                float nextY = a * (pos.x + signX) + b;
                float nextX = ((pos.y + signY) - b) / a;

                Gizmos.color = Color.blue;
                Y = new float2(pos.x + signX, nextY);
                Gizmos.DrawWireSphere(new Vector3(Y.x, 0, Y.y), data.cellSize * 0.2f);

                Gizmos.color = Color.green;
                X = new float2(nextX, pos.y + signY);
                Gizmos.DrawWireSphere(new Vector3(X.x, 0, X.y), data.cellSize * 0.2f);

                if (math.distancesq(Y, current) < math.distancesq(X, current))
                {
                    Gizmos.color = Color.blue;
                    current = Y;
                    currentIndex.x += dir.x > 0 ? 1 : -1;
                    Gizmos.DrawWireSphere(new Vector3(current.x, 0, current.y), data.cellSize * 0.2f);
                }
                else
                {
                    Gizmos.color = Color.green;
                    current = X;
                    currentIndex.y+= dir.y > 0 ? 1 : -1;
                    Gizmos.DrawWireSphere(new Vector3(current.x, 0, current.y), data.cellSize * 0.2f);
                }

            }
        }

        if(lineCastTestTwo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(start.x, 2, start.y), new Vector3(end.x, 2, end.y));
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(new Vector3(circlePos.x, 2, circlePos.y), circleRadius);

            float2 a = start;
            float2 b = end;
            float2 h = circlePos;
            float r = circleRadius;

            bool intersects = SegmentIntersectsCircle(start, end, circlePos, circleRadius);

            if (intersects)
            {
                Gizmos.DrawSphere(new Vector3(circlePos.x, 2, circlePos.y), circleRadius);
            }

        }
    }

     bool SegmentIntersectsCircle(float2 start, float2 end, float2 circle, double r)
    {
        float2 d = end - start;
        float2 f = circle - start;

        float lenSq = math.lengthsq(d);
        float t = (f.x * d.x + f.y * d.y) / lenSq;

        float2 closest;

        if (t < 0)
            closest = start;
        else if (t > 1)
            closest = end;
        else
            closest = new float2(start.x + t * d.x, start.y + t * d.y);

        float2 dist = closest - circle;
        return math.lengthsq(dist) <= r * r;
    }
}
