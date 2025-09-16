using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[InternalBufferCapacity(0)]
public struct RaycastQuerry : IBufferElementData
{
    public float3 start;
    public float3 end;
    public byte mask;

    public bool hit;
    public Entity hitEntity;
    public float3 hitPoint;
}
