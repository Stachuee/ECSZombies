using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TestPathfindingGrid : MonoBehaviour
{

    public Entity entity; 
    public World world;

    DynamicBuffer<PathfindingPoint> pathfindingPoints;
    PathfindingGridDatabase database;

    [SerializeField]
    bool testGrid;

    void Start()
    {
        world = World.DefaultGameObjectInjectionWorld;
    }


    void GetPathfindingGrid()
    {
        EntityManager manager = world.EntityManager;

        EntityQuery query = manager.CreateEntityQuery(typeof(PathfindingGridSingleton));
        NativeArray<Entity> entities = query.ToEntityArray(AllocatorManager.Temp);

        if (entities.Length == 0)
            return;

        Entity targetingEntity = manager.GetComponentData<PathfindingGridSingleton>(entities[0]).targetingSystem;

        pathfindingPoints = manager.GetBuffer<PathfindingPoint>(targetingEntity);
        database = manager.GetComponentData<PathfindingGridDatabase>(targetingEntity);
        
        entities.Dispose();

    }

    private void OnDrawGizmos()
    {
        if (!testGrid || world == null)
            return;

        GetPathfindingGrid();


        for (int y = 0; y < database.gridData.pointCountPerSide; y++)
        {
            for (int x = 0; x < database.gridData.pointCountPerSide; x++)
            {
                int index = PathfindingGrid.GetCellIndexFromCoords(database.gridData, new int2(x, y));
                if (pathfindingPoints[index].obscured > 0)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;

                float2 position = PathfindingGrid.GetWorldFromCoords(database.gridData, new int2(x, y));
                Vector3 worldPosition = new Vector3(position.x, 0, position.y);
                Gizmos.DrawSphere(worldPosition, 0.3f);

                Gizmos.color = Color.black;
                Vector3 walkDirection = new Vector3(pathfindingPoints[index].walkDirection.x, 0, pathfindingPoints[index].walkDirection.y);
                Gizmos.DrawLine(worldPosition, worldPosition + walkDirection);
            }
        }

    }
}
