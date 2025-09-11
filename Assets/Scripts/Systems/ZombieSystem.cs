using System.ComponentModel;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct ZombieSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        ZombieMovmentJob movment = new ZombieMovmentJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        state.Dependency = movment.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public partial struct ZombieMovmentJob : IJobEntity
    {
        public float deltaTime;

        private void Execute(ref Unit unit, ref Health health, ref LocalTransform transform)
        {
            float3 targetVector = float3.zero - transform.Position;
            quaternion target = quaternion.LookRotationSafe(targetVector, math.up());
            transform.Rotation = target;
            float3 forward = math.mul(transform.Rotation, math.forward());
            transform.Position += forward * deltaTime;
        }

    }


    [BurstCompile]
    public partial struct ZombieCollisionJob : IJobEntity
    {
        private void Execute()
        {
        
        }
    }


}
