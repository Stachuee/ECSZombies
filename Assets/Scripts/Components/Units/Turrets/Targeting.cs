using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Targeting : IComponentData
{
    public enum TargetingType : byte
    { 
        Closest, Random, BiggestGrup
    }
    public enum TargetingTeamType : byte
    {
        Friend, Enemy
    }

    public enum FireType : byte
    {
        Direct, Indirect
    }

    public TargetingType targetingType;
    public TargetingTeamType targetingTeamType;
    public FireType fireType;

    public bool targetAvalible;
    public float3 targetPosition;

    public float targetingRange;
    public float timeBetweenUpdates;
    public float nextUpdate;
}
