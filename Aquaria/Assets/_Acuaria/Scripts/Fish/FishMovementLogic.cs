using Acuaria.Environment;
using UnityEngine;
using Random = System.Random;

namespace Acuaria.Fish
{
    public static class FishMovementLogic
    {
        private const float Epsilon = 0.000001f;

        public static void Advance(Fish fish, AquariumBounds bounds,
            FishMovementSettings settings, float deltaTime, Random random)
        {
            Advance(fish, bounds, settings, deltaTime, random, Vector3.zero);
        }

        public static void Advance(Fish fish, AquariumBounds bounds,
            FishMovementSettings settings, float deltaTime, Random random, Vector3 separation)
        {
            if (fish == null || random == null || !float.IsFinite(deltaTime) || deltaTime <= 0f)
            {
                return;
            }

            AquariumBounds safe = CreateSafeBounds(bounds,
                Mathf.Max(settings.WallSafetyDistance, fish.IndividualScale * 0.5f));
            fish.Position = safe.Clamp(fish.Position);
            fish.DirectionTimeRemaining -= deltaTime;
            fish.IsNearWall = IsNearBoundary(fish.Position, safe, settings.EdgeDetectionRadius);
            bool reached = (fish.Destination - fish.Position).sqrMagnitude
                <= settings.EdgeDetectionRadius * settings.EdgeDetectionRadius * 0.25f;
            bool needsEscape = fish.IsNearWall
                && !DestinationLeadsInward(fish.Position, fish.Destination, safe.Center);

            if (fish.DirectionTimeRemaining <= 0f || reached || needsEscape)
            {
                fish.Destination = GenerateDestination(safe, fish.Position, fish.Direction,
                    settings, random);
                fish.DirectionTimeRemaining = NextRange(random,
                    settings.MinimumDirectionTime, settings.MaximumDirectionTime);
                fish.TargetSpeed = NextRange(random, settings.MinimumSpeed, settings.MaximumSpeed);
            }

            float rate = fish.TargetSpeed >= fish.Speed ? settings.Acceleration : settings.Deceleration;
            fish.Speed = Mathf.MoveTowards(ClampSpeed(fish.Speed, settings),
                ClampSpeed(fish.TargetSpeed, settings), rate * deltaTime);

            Vector3 desired = fish.Destination - fish.Position + separation;
            desired = FishOrientationLogic.LimitVerticalDirection(
                desired, settings.MaximumAscentAngle, settings.MaximumDescentAngle);
            fish.Direction = CalculateDirection(fish.Direction, desired,
                settings.TurningSpeed, deltaTime);
            fish.SmoothedDirection = Vector3.Slerp(fish.SmoothedDirection, fish.Direction,
                1f - Mathf.Exp(-settings.TurningSpeed * Mathf.Deg2Rad * deltaTime)).normalized;
            fish.CurrentPitch = Mathf.Atan2(fish.SmoothedDirection.y,
                new Vector2(fish.SmoothedDirection.x, fish.SmoothedDirection.z).magnitude) * Mathf.Rad2Deg;
            fish.Position = CalculatePosition(fish.Position, fish.Direction, fish.Speed, deltaTime, safe);
        }

        public static Vector3 GenerateDestination(AquariumBounds safeBounds, Vector3 position,
            Vector3 direction, float edgeRadius, Random random)
        {
            FishMovementSettings legacy = new FishMovementSettings(
                0.5f, 1f, 1f, 3f, edgeRadius, edgeRadius);
            return GenerateDestination(safeBounds, position, direction, legacy, random);
        }

        public static Vector3 GenerateDestination(AquariumBounds safe, Vector3 position,
            Vector3 direction, FishMovementSettings settings, Random random)
        {
            Vector3 forward = direction.sqrMagnitude > Epsilon ? direction.normalized : Vector3.forward;
            if (IsNearBoundary(position, safe, settings.EdgeDetectionRadius))
            {
                forward = Vector3.Slerp(forward, (safe.Center - position).normalized, 0.78f);
            }

            float yaw = NextRange(random, -32f, 32f);
            Vector3 horizontal = Quaternion.Euler(0f, yaw, 0f) * new Vector3(forward.x, 0f, forward.z);
            if (horizontal.sqrMagnitude < Epsilon)
            {
                horizontal = Vector3.forward;
            }

            float preferredMin = Mathf.Lerp(safe.Min.y, safe.Max.y, settings.PreferredDepthMinimum);
            float preferredMax = Mathf.Lerp(safe.Min.y, safe.Max.y, settings.PreferredDepthMaximum);
            float verticalMin = Mathf.Max(preferredMin, position.y - settings.MaximumVerticalVariation);
            float verticalMax = Mathf.Min(preferredMax, position.y + settings.MaximumVerticalVariation);
            if (verticalMin > verticalMax)
            {
                verticalMin = verticalMax = Mathf.Clamp(position.y, preferredMin, preferredMax);
            }

            float distance = NextRange(random,
                settings.EdgeDetectionRadius * 1.75f, settings.EdgeDetectionRadius * 4f);
            Vector3 destination = position + horizontal.normalized * distance;
            destination.y = NextRange(random, verticalMin, verticalMax);
            destination = safe.Clamp(destination);
            if ((destination - position).sqrMagnitude < settings.EdgeDetectionRadius * 0.25f)
            {
                destination = safe.Clamp(Vector3.Lerp(position, safe.Center, 0.55f));
            }

            return destination;
        }

        public static Vector3 CalculateDirection(Vector3 current, Vector3 position,
            Vector3 destination, float speed, float turningRadius, float deltaTime)
        {
            return CalculateDirection(current, destination - position,
                Mathf.Rad2Deg * Mathf.Max(0.01f, speed) / Mathf.Max(0.01f, turningRadius), deltaTime);
        }

        public static Vector3 CalculateDirection(
            Vector3 current, Vector3 desired, float turningSpeed, float deltaTime)
        {
            if (desired.sqrMagnitude < Epsilon)
            {
                return current.sqrMagnitude > Epsilon ? current.normalized : Vector3.forward;
            }
            Vector3 from = current.sqrMagnitude > Epsilon ? current.normalized : desired.normalized;
            return Vector3.RotateTowards(from, desired.normalized,
                Mathf.Max(1f, turningSpeed) * Mathf.Deg2Rad * Mathf.Max(0f, deltaTime), 0f).normalized;
        }

        public static Vector3 CalculatePosition(Vector3 position, Vector3 direction,
            float speed, float deltaTime, AquariumBounds safe)
        {
            if (!float.IsFinite(deltaTime) || deltaTime <= 0f)
            {
                return safe.Clamp(position);
            }
            Vector3 normalized = direction.sqrMagnitude > Epsilon ? direction.normalized : Vector3.forward;
            return safe.Clamp(position + normalized * Mathf.Max(0f, speed) * deltaTime);
        }

        public static float ClampSpeed(float speed, FishMovementSettings settings)
        {
            return float.IsFinite(speed)
                ? Mathf.Clamp(speed, settings.MinimumSpeed, settings.MaximumSpeed)
                : settings.MinimumSpeed;
        }

        public static AquariumBounds CreateSafeBounds(AquariumBounds bounds, float margin)
        {
            Vector3 size = bounds.Size - Vector3.one * (Mathf.Max(0f, margin) * 2f);
            size = new Vector3(Mathf.Max(0.001f, size.x), Mathf.Max(0.001f, size.y), Mathf.Max(0.001f, size.z));
            return new AquariumBounds(bounds.Center, size);
        }

        private static bool IsNearBoundary(Vector3 position, AquariumBounds bounds, float radius)
        {
            Vector3 a = position - bounds.Min;
            Vector3 b = bounds.Max - position;
            float minimum = Mathf.Min(Mathf.Min(a.x, Mathf.Min(a.y, a.z)),
                Mathf.Min(b.x, Mathf.Min(b.y, b.z)));
            return minimum <= Mathf.Max(0f, radius);
        }

        private static bool DestinationLeadsInward(Vector3 position, Vector3 destination, Vector3 center)
        {
            Vector3 a = destination - position;
            Vector3 b = center - position;
            return a.sqrMagnitude > Epsilon && b.sqrMagnitude > Epsilon
                && Vector3.Dot(a.normalized, b.normalized) > 0.25f;
        }

        private static float NextRange(Random random, float min, float max)
        {
            return Mathf.Lerp(min, max, (float)random.NextDouble());
        }
    }
}
