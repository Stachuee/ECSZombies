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


}
