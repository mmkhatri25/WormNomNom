using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(DuckMovementSystem))]
public partial class DuckAnimationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float move = Input.GetAxisRaw("Horizontal");
        foreach (var (input, entity) in SystemAPI.Query<RefRO<PlayerInputData>>().WithEntityAccess())
        {
            Animator animator = EntityManager.HasComponent<Animator>(entity)
                ? EntityManager.GetComponentObject<Animator>(entity)
                : null;

            if (animator != null)
            {
                float absMove = Mathf.Abs(move);

                // Set animation parameter
                animator.SetFloat("Speed", absMove);

                // Rotate duck based on direction
                if (absMove > 0.01f)
                {
                    float targetY = move > 0 ? 145f : -145f;
                    animator.transform.rotation = Quaternion.Euler(0, targetY, 0);
                }
            }
        }
    }

}

