using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct PhysicBodyMask
{
    public int mask;
    public enum MaskType { Enemy, Building, Terrain }

    public static PhysicBodyMask operator ~(PhysicBodyMask operand) => new PhysicBodyMask{
        mask = ~operand.mask 
    };

    public static bool IgnoreCollision(PhysicBodyMask mask1, PhysicBodyMask mask2)
    {
        return (mask1.mask & mask2.mask) == 0;
    }

    public static void SetMask(ref PhysicBodyMask mask, MaskType type)
    {
        mask.mask |= type switch
        {
            MaskType.Enemy => GetEnemyMask(),
            MaskType.Building => GetBuildingMask(),
            MaskType.Terrain => GetTerrainMask(),
            _ => throw new Exception("Forgot to add type")
        };
    }
    public static void ResetMask(ref PhysicBodyMask mask, MaskType type)
    {
        mask.mask &= type switch
        {
            MaskType.Enemy => ~GetEnemyMask(),
            MaskType.Building => ~GetBuildingMask(),
            MaskType.Terrain => ~GetTerrainMask(),
            _ => throw new Exception("Forgot to add type")
        };
    }

    public static int GetEnemyMask() => 1;
    public static int GetBuildingMask() => 2;
    public static int GetTerrainMask() => 4;
}

public struct PhysicBody : IComponentData
{
    public bool staticBody;

    public float radius;
    public float height;
    public float mass;

    public PhysicBodyMask mask;

    public float3 velocity;
}
