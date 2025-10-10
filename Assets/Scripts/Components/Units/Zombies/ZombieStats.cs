using Unity.Entities;
using UnityEngine;

public struct ZombieStats :IComponentData
{
    public float speed;
    public float damage;

    public bool innitialized;
}
