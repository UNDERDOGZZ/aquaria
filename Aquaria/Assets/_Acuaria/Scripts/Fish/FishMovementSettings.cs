using UnityEngine;

namespace Acuaria.Fish
{
    public readonly struct FishMovementSettings
    {
        public FishMovementSettings(float minimumSpeed, float maximumSpeed,
            float minimumDirectionTime, float maximumDirectionTime, float turningRadius, float size)
            : this(size, size, minimumSpeed, maximumSpeed, 0.35f, 0.5f,
                Mathf.Rad2Deg * maximumSpeed / Mathf.Max(0.01f, turningRadius),
                minimumDirectionTime, maximumDirectionTime, size, 0.15f, 0.85f,
                size, 18f, 15f, size, 0.5f, 0.35f, 4f, 1f)
        {
        }

        public FishMovementSettings(float minimumScale, float maximumScale,
            float minimumSpeed, float maximumSpeed, float acceleration, float deceleration,
            float turningSpeed, float minimumDirectionTime, float maximumDirectionTime,
            float wallSafetyDistance, float preferredDepthMinimum, float preferredDepthMaximum,
            float maximumVerticalVariation, float maximumAscentAngle, float maximumDescentAngle,
            float separationRadius, float separationStrength, float maximumSeparation,
            float maximumVisualBank, float swimOscillation)
        {
            MinimumScale = Mathf.Max(0.01f, Mathf.Min(minimumScale, maximumScale));
            MaximumScale = Mathf.Max(MinimumScale, Mathf.Max(minimumScale, maximumScale));
            MinimumSpeed = Mathf.Max(0.01f, Mathf.Min(minimumSpeed, maximumSpeed));
            MaximumSpeed = Mathf.Max(MinimumSpeed, Mathf.Max(minimumSpeed, maximumSpeed));
            Acceleration = Mathf.Max(0.01f, acceleration);
            Deceleration = Mathf.Max(0.01f, deceleration);
            TurningSpeed = Mathf.Max(1f, turningSpeed);
            MinimumDirectionTime = Mathf.Max(0.1f, Mathf.Min(minimumDirectionTime, maximumDirectionTime));
            MaximumDirectionTime = Mathf.Max(MinimumDirectionTime, Mathf.Max(minimumDirectionTime, maximumDirectionTime));
            WallSafetyDistance = Mathf.Max(0.01f, wallSafetyDistance);
            PreferredDepthMinimum = Mathf.Clamp01(Mathf.Min(preferredDepthMinimum, preferredDepthMaximum));
            PreferredDepthMaximum = Mathf.Clamp01(Mathf.Max(preferredDepthMinimum, preferredDepthMaximum));
            MaximumVerticalVariation = Mathf.Max(0.01f, maximumVerticalVariation);
            MaximumAscentAngle = Mathf.Clamp(maximumAscentAngle, 0f, 45f);
            MaximumDescentAngle = Mathf.Clamp(maximumDescentAngle, 0f, 45f);
            SeparationRadius = Mathf.Max(0.01f, separationRadius);
            SeparationStrength = Mathf.Max(0f, separationStrength);
            MaximumSeparation = Mathf.Max(0f, maximumSeparation);
            MaximumVisualBank = Mathf.Clamp(maximumVisualBank, 0f, 15f);
            SwimOscillation = Mathf.Max(0f, swimOscillation);
        }

        public float MinimumScale { get; }
        public float MaximumScale { get; }
        public float MinimumSpeed { get; }
        public float MaximumSpeed { get; }
        public float Acceleration { get; }
        public float Deceleration { get; }
        public float TurningSpeed { get; }
        public float MinimumDirectionTime { get; }
        public float MaximumDirectionTime { get; }
        public float WallSafetyDistance { get; }
        public float PreferredDepthMinimum { get; }
        public float PreferredDepthMaximum { get; }
        public float MaximumVerticalVariation { get; }
        public float MaximumAscentAngle { get; }
        public float MaximumDescentAngle { get; }
        public float SeparationRadius { get; }
        public float SeparationStrength { get; }
        public float MaximumSeparation { get; }
        public float MaximumVisualBank { get; }
        public float SwimOscillation { get; }
        public float BodyMargin => MaximumScale * 0.5f;
        public float EdgeDetectionRadius => Mathf.Max(WallSafetyDistance, MaximumScale);
        public float TurningRadius => MaximumSpeed / (TurningSpeed * Mathf.Deg2Rad);
        public float Size => (MinimumScale + MaximumScale) * 0.5f;
    }
}
