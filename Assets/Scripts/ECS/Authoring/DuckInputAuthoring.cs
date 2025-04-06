using Unity.Entities;
using UnityEngine;

public class DuckInputAuthoring : MonoBehaviour
{
    class Baker : Baker<DuckInputAuthoring>
    {
        public override void Bake(DuckInputAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerInputData>(entity);
        }
    }
}