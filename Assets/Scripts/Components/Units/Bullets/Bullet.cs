using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Bullet : IComponentData
{
    public float3 direction;
    public float speed;
    public float lifetime;
    public float damage;
    public bool affectedByGravity;

    public bool dispose;
}
