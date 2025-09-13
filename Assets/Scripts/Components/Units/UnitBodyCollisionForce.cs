using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct UnitBodyCollisionForce : IComponentData
{
    public float2 collisionForce;
}
