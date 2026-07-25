using UnityEngine;

namespace Acuaria.Fish
{
    public readonly struct FishSpawnPlan
    {
        public FishSpawnPlan(FishSpecies species, Vector3 position, Vector3 direction,
            float scale, float speed, int seed, string instanceId)
        {
            Species = species;
            Position = position;
            Direction = direction;
            Scale = scale;
            Speed = speed;
            Seed = seed;
            InstanceId = instanceId;
        }
        public FishSpecies Species { get; }
        public Vector3 Position { get; }
        public Vector3 Direction { get; }
        public float Scale { get; }
        public float Speed { get; }
        public int Seed { get; }
        public string InstanceId { get; }
    }
}
