using Unity.Entities;
using UnityEngine;

public struct Turret : IComponentData
{
    public float firerate;

    public bool rotate;
    public float turretRotationSharpness;
    public float maxShootAngle;

    public bool canShoot;

    public float lastShot;

}
