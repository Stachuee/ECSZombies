using Unity.Entities;
using UnityEngine;

public struct Health : IComponentData
{
    public float CurrentHealth;
    public bool IsDead => CurrentHealth <= 0;
}
