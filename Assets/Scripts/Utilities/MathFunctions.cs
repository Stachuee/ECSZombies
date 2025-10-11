using System.Drawing;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct MathFunctions
{
    public static bool CheckIfPointInsidePolygon(ref NativeArray<float2> borders, float2 point)
    {
        double minX = borders[0].x;
        double maxX = borders[0].x;
        double minY = borders[0].y;
        double maxY = borders[0].y;
        for (int i = 1; i < borders.Length; i++)
        {
            float2 q = borders[i];
            minX = math.min(q.x, minX);
            maxX = math.max(q.x, maxX);
            minY = math.min(q.y, minY);
            maxY = math.max(q.y, maxY);
        }

        if (point.x < minX || point.x > maxX || point.y < minY || point.y > maxY)
        {
            return false;
        }
         
        bool inside = false;
        for (int i = 0, j = borders.Length - 1; i < borders.Length; j = i++)
        {
            if ((borders[i].y > point.y) != (borders[j].y > point.y) &&
                 point.x < (borders[j].x - borders[i].x) * (point.y - borders[i].y) / (borders[j].y - borders[i].y) + borders[i].x)
            {
                inside = !inside;
            }
        }

        return inside;
    }

  
}
