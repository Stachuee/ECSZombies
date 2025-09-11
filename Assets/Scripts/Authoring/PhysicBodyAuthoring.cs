using Unity.Entities;
using UnityEngine;

public class BodyAuthoring : MonoBehaviour
{
    [SerializeField]
    float radius;
    [SerializeField]
    float height;

    class Baker : Baker<BodyAuthoring>
    {
        public override void Bake(BodyAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            PhysicBody body = new PhysicBody();
            
            AddComponent(entity, body);
        }
    }
}
