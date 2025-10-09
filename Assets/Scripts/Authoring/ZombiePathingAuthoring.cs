using Unity.Entities;
using UnityEngine;

public class ZombiePathingAuthoring : MonoBehaviour
{
    public class Baker : Baker<ZombiePathingAuthoring>
    {
        public override void Bake(ZombiePathingAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombiePathing
            {
                
            });
        }
    }
}
