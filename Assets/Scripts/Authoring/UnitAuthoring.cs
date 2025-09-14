using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    public byte team;
    class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new Unit
            {
                team = authoring.team,
            });
        }
    }
}
