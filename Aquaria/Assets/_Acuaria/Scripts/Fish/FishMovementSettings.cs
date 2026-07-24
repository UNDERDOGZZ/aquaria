using UnityEngine;

namespace Acuaria.Fish
{
    public readonly struct FishMovementSettings
    {
        public FishMovementSettings(
            float minimumSpeed,
            float maximumSpeed,
            float minimumDirectionTime,
            float maximumDirectionTime,
            float turningRadius,
            float size)
        {
            MinimumSpeed = Mathf.Max(0.01f, Mathf.Min(minimumSpeed, maximumSpeed));
            MaximumSpeed = Mathf.Max(MinimumSpeed, Mathf.Max(minimumSpeed, maximumSpeed));
            MinimumDirectionTime = Mathf.Max(0.1f, Mathf.Min(minimumDirectionTime, maximumDirectionTime));
            MaximumDirectionTime = Mathf.Max(
                MinimumDirectionTime,
                Mathf.Max(minimumDirectionTime, maximumDirectionTime));
            TurningRadius = Mathf.Max(0.01f, turningRadius);
            Size = Mathf.Max(0.01f, size);
        }

        public float MinimumSpeed { get; }
        public float MaximumSpeed { get; }
        public float MinimumDirectionTime { get; }
        public float MaximumDirectionTime { get; }
        public float TurningRadius { get; }
        public float Size { get; }
        public float BodyMargin => Size * 0.5f;
        public float EdgeDetectionRadius => Mathf.Max(Size, TurningRadius * 1.25f);
    }
}
