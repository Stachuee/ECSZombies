using Unity.Entities;
using UnityEngine;

public struct TurretsConfig : IComponentData
{
    public Entity simpleBullet;

    public float bulletLifeTime;

}
