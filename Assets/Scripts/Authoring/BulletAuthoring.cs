using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    [SerializeField]
    float damage;
    [SerializeField]
    float speed;
    [SerializeField]
    LayerMask mask;

    public class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new Bullet
            {
                damage = authoring.damage,
                speed = authoring.speed,
                direction = new float3(1, 0, 0),
                hitMask = new PhysicBodyMask()
                {
                    mask = authoring.mask.value
                }
            });
        }
    }
}
