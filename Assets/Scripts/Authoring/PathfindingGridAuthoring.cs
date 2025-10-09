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
            TargetingGridDatabase database = new TargetingGridDatabase();
            DynamicBuffer<PathfindingPoint> points = AddBuffer<PathfindingPoint>(entity);

            TargetingGridDatabase.CreataDatabase(1, 1, ref database, ref points);
            AddComponent(entity, database);
        }
    }

}
