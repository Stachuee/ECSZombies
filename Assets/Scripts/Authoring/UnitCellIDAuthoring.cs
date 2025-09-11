using Unity.Entities;
using UnityEngine;

public class UnitCellIDAuthoring : MonoBehaviour
{
    class Baker : Baker<UnitCellIDAuthoring>
    {
        public override void Bake(UnitCellIDAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            UnitCellID unitCellId = new UnitCellID();
            AddComponent(entity, unitCellId);
        }
    }

}
