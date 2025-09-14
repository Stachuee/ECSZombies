using Unity.Entities;
using UnityEngine;

public class TurretAuthoring : MonoBehaviour
{
    [SerializeField]
    float turretRotationSharpness;
    [SerializeField]
    float maxShootAngle;
    [SerializeField]
    bool rotateTurret;

    class Baker : Baker<TurretAuthoring>
    {
        public override void Bake(TurretAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new Turret
            {
                turretRotationSharpness = authoring.turretRotationSharpness,
                maxShootAngle = authoring.maxShootAngle,
                rotate = authoring.rotateTurret
            });
        }
    }
}
