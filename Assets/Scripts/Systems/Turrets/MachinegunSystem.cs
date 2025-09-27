using Unity.Burst;
using Unity.Burst.Intrinsics;
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

        ShootJob shootJob = new ShootJob
        {
            ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged),
            time = (float)SystemAPI.Time.ElapsedTime,
            bulletPrefab = turretConfig.simpleBullet
        };

        state.Dependency = shootJob.Schedule(state.Dependency);

    }


    public partial struct ShootJob : IJobEntity, IJobEntityChunkBeginEnd
    {
        public EntityCommandBuffer ecb;
        public float time;
        public Entity bulletPrefab;

        private int _chunkIndex;

        public void Execute(ref Turret turret, in LocalTransform lt)
        {
            if(turret.canShoot && turret.lastShot + 60 / turret.firerate < time)
            {
                Entity bullet = ecb.Instantiate(bulletPrefab);
                PhysicBodyMask mask = new PhysicBodyMask();
                PhysicBodyMask.SetMask(ref mask, PhysicBodyMask.MaskType.Enemy);
                PhysicBodyMask.SetMask(ref mask, PhysicBodyMask.MaskType.Terrain);

                ecb.SetComponent(bullet, LocalTransform.FromPositionRotation(lt.Position, lt.Rotation));
                ecb.SetComponent(bullet, new Bullet
                {
                    damage = 10,
                    direction = lt.Forward(),
                    speed = 100,
                    lifetimeRemain = 5,
                    hitMask = mask
                });
                turret.lastShot = time;
            }
        }

        public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            _chunkIndex = unfilteredChunkIndex;
            return true;
        }

        public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask,
            bool chunkWasExecuted)
        { }
    }
}
