using Unity.Entities;
using UnityEngine;

public class TurretsConfigAuthoring : MonoBehaviour
{
    [Header("Bullets prefabs")]
    [SerializeField]
    GameObject simpleBullet;

    [Header("Bullets settings")]
    [SerializeField]
    float bulletLifeTime;


    public class Baker : Baker<TurretsConfigAuthoring>
    {
        public override void Bake(TurretsConfigAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new TurretsConfig
            {
                simpleBullet = GetEntity(authoring.simpleBullet, TransformUsageFlags.Dynamic),

                bulletLifeTime = authoring.bulletLifeTime,
            });
        }
    }
}
