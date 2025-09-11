using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GridDatabaseAuthoring : MonoBehaviour
{
    [SerializeField]
    float halfSize;
    [SerializeField]
    int cellsPerSide;
    [SerializeField]
    int elementsPerCell;
    class Baker : Baker<GridDatabaseAuthoring>
    {
        public override void Bake(GridDatabaseAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            GridDatabase gridDatabase = new GridDatabase();
            DynamicBuffer<GridCell> cells = AddBuffer<GridCell>(entity);
            DynamicBuffer<GridCellElement> elements = AddBuffer<GridCellElement>(entity);

            GridDatabase.CreataDatabase(authoring.halfSize, authoring.cellsPerSide, authoring.elementsPerCell, ref gridDatabase, ref cells, ref elements);
            AddComponent(entity, gridDatabase);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(default, new float3(halfSize * 2, 1, halfSize * 2));

        for(int i = 0; i < cellsPerSide; i++)
        {
            float z = -halfSize + (halfSize * 2 / cellsPerSide) * i;
            Gizmos.DrawLine(new float3(-halfSize, 0, z), new float3(halfSize, 0, z));
            Gizmos.DrawLine(new float3(z, 0, -halfSize), new float3(z, 0, halfSize));
        }
    }
}
