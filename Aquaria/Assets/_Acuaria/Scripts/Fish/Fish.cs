using UnityEngine;

namespace Acuaria.Fish
{
    public sealed class Fish
    {
        public Fish(Vector3 position, Vector3 direction, float speed,
            Vector3 destination, float directionTimeRemaining)
            : this("legacy-fish", null, position, direction, speed, speed, destination,
                1f, 0, directionTimeRemaining)
        {
        }

        public Fish(string instanceId, FishSpecies species, Vector3 position, Vector3 direction,
            float speed, float targetSpeed, Vector3 destination, float individualScale,
            int randomSeed, float directionTimeRemaining)
        {
            InstanceId = instanceId;
            Species = species;
            Position = position;
            Direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector3.forward;
            SmoothedDirection = Direction;
            Speed = speed;
            TargetSpeed = targetSpeed;
            Destination = destination;
            IndividualScale = individualScale;
            RandomSeed = randomSeed;
            DirectionTimeRemaining = directionTimeRemaining;
        }

        public string InstanceId { get; }
        public FishSpecies Species { get; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 SmoothedDirection { get; set; }
        public float Speed { get; set; }
        public float TargetSpeed { get; set; }
        public Vector3 Destination { get; set; }
        public float IndividualScale { get; }
        public int RandomSeed { get; }
        public float DirectionTimeRemaining { get; set; }
        public float CurrentPitch { get; set; }
        public float VisualBank { get; set; }
        public bool IsNearWall { get; set; }
    }
}
