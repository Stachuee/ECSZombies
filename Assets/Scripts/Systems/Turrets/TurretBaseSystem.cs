using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(BuildingActions)), UpdateAfter(typeof(TargetingSystem))]
public partial struct TurretBaseSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        RotationJob rotationjob = new RotationJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        state.Dependency = rotationjob.ScheduleParallel(state.Dependency);
    }

    public partial struct RotationJob : IJobEntity
    {
        public float deltaTime;

        public void Execute(ref Targeting targeting, ref LocalTransform lt, ref Turret turret)
        {
            if(!turret.rotate)
            {
                turret.canShoot = true;
                return;
            }

            float3 posA = new float3(targeting.targetPosition.x, 0, targeting.targetPosition.z);
            float3 posB = new float3(lt.Position.x, 0, lt.Position.z);
            
            quaternion direction = quaternion.LookRotationSafe(posA - posB, math.up());

            turret.canShoot = math.angle(direction, lt.Rotation) * math.TODEGREES < turret.maxShootAngle;

            lt.Rotation = math.slerp(lt.Rotation, direction, GetSharpnessInterpolant(turret.turretRotationSharpness, deltaTime));

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSharpnessInterpolant(float sharpness, float dt)
        {
            return math.saturate(1f - math.exp(-sharpness * dt));
        }
    }
}
