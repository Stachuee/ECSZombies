using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PhysicBody : IComponentData
{
    public bool staticBody;

    public float radius;
    public float height;

    public float3 velocity;
}
