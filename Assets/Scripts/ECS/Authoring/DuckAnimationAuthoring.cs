using System;
using Unity.Entities;
using UnityEngine;

public class DuckAnimationAuthoring : MonoBehaviour
{
    public Animator animator;

    class Baker : Baker<DuckAnimationAuthoring>
    {
        public override void Bake(DuckAnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AnimationData
            {
                AnimatorEntity = entity,
                LastHorizontal = 0f
            });

            AddComponentObject(entity, authoring.animator); // 👈 Required for hybrid Animator
        }
    }
}