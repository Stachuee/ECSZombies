using Unity.Entities;
using UnityEngine;

public struct PathfindingGridSingleton : IComponentData
{
    public Entity targetingSystem;
}
