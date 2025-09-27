using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Bullet : IComponentData
{
    public float3 direction;
    public float speed;
    public float lifetimeRemain;
    public float damage;
    public bool affectedByGravity;

    public PhysicBodyMask hitMask;

    public bool dispose;
}
