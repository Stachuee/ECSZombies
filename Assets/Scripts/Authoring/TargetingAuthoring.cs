using Unity.Entities;
using UnityEngine;

public class TargetingAuthoring : MonoBehaviour
{

    public float range;
    public float updateTimer;
    public Targeting.TargetingType targetingType;
    public Targeting.TargetingTeamType targetingTeamType;

    class Baker : Baker<TargetingAuthoring>
    {
        public override void Bake(TargetingAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new Targeting
            {
                targetAvalible = false,
                nextUpdate = Random.Range(0, 0.5f),
                targetingRange = authoring.range,
                targetingType = authoring.targetingType,
                timeBetweenUpdates = authoring.updateTimer,
                targetingTeamType = authoring.targetingTeamType
            });
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
