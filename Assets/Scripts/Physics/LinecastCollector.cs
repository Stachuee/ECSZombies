using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;


public struct LinecastCollector : IGridCollector
{
    public float3 start;
    public float3 end;
    public byte mask;

    public bool hit;
    public Entity hitEntity;
    

    public void OnVisitCell(in GridCell cell, in UnsafeList<GridCellElement> elements, out bool exitEarly)
    {
        exitEarly = false;
        for (int i = cell.startIndex; i < cell.startIndex + cell.elementCount; i++)
        {
            GridCellElement element = elements[i];
            if (SegmentIntersectsCircle(start.xz, end.xz, element.postion.xz, element.radius))
            {
                hit = true;
                hitEntity = element.entity;
                exitEarly = true;
            }
        }
    }

    bool SegmentIntersectsCircle(float2 start, float2 end, float2 circle, double r)
    {
        float2 d = end - start;
        float2 f = circle - start;

        float lenSq = math.lengthsq(d);
        float t = (f.x * d.x + f.y * d.y) / lenSq;

        float2 closest;

        if (t < 0)
            closest = start;
        else if (t > 1)
            closest = end;
        else
            closest = new float2(start.x + t * d.x, start.y + t * d.y);

        float2 dist = closest - circle;
        return math.lengthsq(dist) <= r * r;
    }
}
