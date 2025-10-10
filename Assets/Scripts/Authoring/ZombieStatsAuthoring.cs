using Unity.Entities;
using UnityEngine;

public class ZombieStatsAuthoring : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float damage;

    public class Baker : Baker<ZombieStatsAuthoring>
    {
        public override void Bake(ZombieStatsAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieStats()
            {
                speed = authoring.speed,
                damage = authoring.damage,

                innitialized = false
            });
        }
    }

}
