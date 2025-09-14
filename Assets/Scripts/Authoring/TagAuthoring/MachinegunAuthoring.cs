using Unity.Entities;
using UnityEngine;

public class MachinegunAuthoring : MonoBehaviour
{
    public class Baker : Baker<MachinegunAuthoring>
    {
        public override void Bake(MachinegunAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Machinegun());
        }
    }

}
