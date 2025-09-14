using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

public struct GridTargetCollector : IGridCollector
{
    public Targeting.TargetingType targetingType;
    public Targeting.TargetingTeamType targetingTeamType;

    public float targetingRange;
    public byte team;
    public float3 position;

    public bool foundAvalibleTarget;
    public float3 targetPosition;

    public float closest;
    public int closestId;

    public void OnVisitCell(in GridCell cell, in UnsafeList<GridCellElement> elements, out bool exitEarly)
    {
        exitEarly = false;
        foundAvalibleTarget = false;

        closest = float.MaxValue;

        for(int i = cell.startIndex; i < cell.startIndex + cell.elementCount; i++)
        {
            GridCellElement element = elements[i];

            if ((element.team == team && targetingTeamType == Targeting.TargetingTeamType.Enemy) || (element.team != team && targetingTeamType == Targeting.TargetingTeamType.Friend))
                continue;

            float newDist = GridData.GetRealDistanceSq(position, element.postion);
            
            if(closest > newDist)
            {
                closest = newDist;
                closestId = i;            
            }
        }

        if(closest <= targetingRange * targetingRange)
        {
            foundAvalibleTarget = true;
            targetPosition = elements[closestId].postion;
        }
    }
}
