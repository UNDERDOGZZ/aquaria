using UnityEngine;

namespace Acuaria.Fish
{
    public sealed class Fish
    {
        public Fish(
            Vector3 position,
            Vector3 direction,
            float speed,
            Vector3 destination,
            float directionTimeRemaining)
        {
            Position = position;
            Direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector3.forward;
            Speed = speed;
            Destination = destination;
            DirectionTimeRemaining = directionTimeRemaining;
        }

        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float Speed { get; set; }
        public Vector3 Destination { get; set; }
        public float DirectionTimeRemaining { get; set; }
    }
}
