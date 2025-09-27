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

    [SerializeField]
    PhysicBodyMask.MaskType maskType;

    class Baker : Baker<BodyAuthoring>
    {
        public override void Bake(BodyAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            PhysicBodyMask mask = new PhysicBodyMask();
            PhysicBodyMask.SetMask(ref mask, authoring.maskType);

            PhysicBody body = new PhysicBody
            {
                height = authoring.height,
                radius = authoring.radius,
                staticBody = authoring.staticBody,
                mask = mask
            };
            UnitBodyCollisionForce force = new UnitBodyCollisionForce();

            AddComponent(entity, body);
            AddComponent(entity, force);
        }
    }
}
