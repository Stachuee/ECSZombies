using Unity.Entities;
using UnityEngine;

public struct PhysicsCastDatabase : IComponentData
{
    public int capacity;
    public int count;

    public static void CreateDatabase(ref PhysicsCastDatabase database, int baseCapacity, ref DynamicBuffer<RaycastQuerry> querries)
    {
        querries.Clear();

        querries.Capacity = baseCapacity;

        querries.Resize(baseCapacity, Unity.Collections.NativeArrayOptions.ClearMemory);
    }
}
