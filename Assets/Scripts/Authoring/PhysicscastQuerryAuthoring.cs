using Unity.Entities;
using UnityEngine;

public class PhysicscastQuerryAuthoring : MonoBehaviour
{
    [SerializeField]
    int baseSize;

    public class Baker : Baker<PhysicscastQuerryAuthoring>
    {
        public override void Bake(PhysicscastQuerryAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);

            PhysicsCastDatabase database = new PhysicsCastDatabase()
            {

            };
            DynamicBuffer<RaycastQuerry> buffer = AddBuffer<RaycastQuerry>(entity);

            PhysicsCastDatabase.CreateDatabase(ref database, authoring.baseSize, ref buffer);
            AddComponent(entity, database);
        }
    }
}
