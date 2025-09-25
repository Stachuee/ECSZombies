using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(BuildingActions)), UpdateAfter(typeof(TurretBaseSystem))]
public partial struct MachinegunSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        TurretsConfig turretConfig = SystemAPI.GetSingleton<TurretsConfig>();

    }


    public partial struct ShootJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public float time;

        public void Execute(ref Turret turret, in LocalTransform lt)
        {
            if(turret.canShoot && turret.lastShot + 60 / turret.firerate < time)
            {
                turret.lastShot = time;
            }
        }
    }
}
