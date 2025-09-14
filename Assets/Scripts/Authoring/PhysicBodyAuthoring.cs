using Unity.Entities;
using UnityEngine;

public class BodyAuthoring : MonoBehaviour
{
    [SerializeField]
    float radius;
    [SerializeField]
    float height;
    [SerializeField]
    bool staticBody;

    class Baker : Baker<BodyAuthoring>
    {
        public override void Bake(BodyAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            PhysicBody body = new PhysicBody
            {
                height = authoring.height,
                radius = authoring.radius,
                staticBody = authoring.staticBody
            };
            UnitBodyCollisionForce force = new UnitBodyCollisionForce();

            AddComponent(entity, body);
            AddComponent(entity, force);
        }
    }
}
