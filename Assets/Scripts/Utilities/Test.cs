using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Test : MonoBehaviour
{

    public int pointCountPerSide = 10;
    public float halfSize = 5;

    public List<float2> sides;

    public float2 point;

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //for (int i = 0, j = sides.Count - 1; i < sides.Count; j = i++)
        //{
        //    Gizmos.DrawLine(new Vector3(sides[i].x, 0, sides[i].y), new Vector3(sides[j].x, 0, sides[j].y));
        //}


        //NativeArray<float2> borders = new NativeArray<float2>(sides.Count, Allocator.Temp);

        //borders.CopyFrom(sides.ToArray());

        //if(MathFunctions.CheckIfPointInsidePolygon(ref borders, point))
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawWireSphere(new Vector3(point.x, 0, point.y), 0.5f);
        //}
        //else
        //{
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawWireSphere(new Vector3(point.x, 0, point.y), 0.5f);
        //}

        //    borders.Dispose();
    }

    //private void OnDrawGizmosSelected()
    //{
    //    TargetingGrid grid = new TargetingGrid(halfSize, pointCountPerSide);
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawWireSphere(new Vector3(pos.x, 0, pos.y), 0.2f);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(default, new float3(halfSize * 2, 1, halfSize * 2));

    //    int2 coords = TargetingGrid.GetCoordsFromWorld(grid, pos);

    //    for (int i = 0; i < pointCountPerSide; i++)
    //    {
    //        for (int j = 0; j < pointCountPerSide; j++)
    //        {
    //            float x = -grid.halfSize + grid.pointsDistnace * j;
    //            float y = -grid.halfSize + grid.pointsDistnace * i;
    //            if (i == coords.y && j == coords.x)
    //                Gizmos.color = Color.blue;
    //            else
    //                Gizmos.color = Color.red;
    //            Gizmos.DrawSphere(new Vector3(x, 0, y), 0.1f);
    //        }
    //    }

    //}
}
