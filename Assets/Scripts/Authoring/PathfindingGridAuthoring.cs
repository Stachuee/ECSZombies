using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PathfindingGridAuthoring : MonoBehaviour
{
    class Baker : Baker<PathfindingGridAuthoring>
    {
        public override void Bake(PathfindingGridAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            PathfindingGridDatabase database = new PathfindingGridDatabase();
            DynamicBuffer<PathfindingPoint> points = AddBuffer<PathfindingPoint>(entity);

            PathfindingGridDatabase.CreataDatabase(1, 1, ref database, ref points);
            AddComponent(entity, database);
        }
    }

}
