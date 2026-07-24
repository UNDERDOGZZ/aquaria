using System;
using Acuaria.Environment;
using UnityEngine;
using Random = System.Random;

namespace Acuaria.Fish
{
    public static class FishMovementLogic
    {
        private const float MinimumDeltaTime = 0.000001f;
        private const float MaximumHeadingVariation = 28f;
        private const float MaximumVerticalVariation = 10f;

        public static void Advance(
            Fish fish,
            AquariumBounds aquariumBounds,
            FishMovementSettings settings,
            float deltaTime,
            Random random)
        {
            if (fish == null || random == null || !float.IsFinite(deltaTime) || deltaTime <= 0f)
            {
                return;
            }

            AquariumBounds safeBounds = CreateSafeBounds(aquariumBounds, settings.BodyMargin);
            fish.Position = safeBounds.Clamp(fish.Position);
            fish.DirectionTimeRemaining -= deltaTime;

            bool destinationReached =
                Vector3.SqrMagnitude(fish.Destination - fish.Position)
                <= settings.EdgeDetectionRadius * settings.EdgeDetectionRadius * 0.25f;
            bool requiresBoundaryAvoidance =
                IsNearBoundary(fish.Position, safeBounds, settings.EdgeDetectionRadius)
                && !DestinationLeadsInward(fish.Position, fish.Destination, safeBounds.Center);

            if (fish.DirectionTimeRemaining <= 0f
                || destinationReached
                || requiresBoundaryAvoidance)
            {
                fish.Destination = GenerateDestination(
                    safeBounds,
                    fish.Position,
                    fish.Direction,
                    settings.EdgeDetectionRadius,
                    random);
                fish.DirectionTimeRemaining = NextRange(
                    random,
                    settings.MinimumDirectionTime,
                    settings.MaximumDirectionTime);
                fish.Speed = NextRange(random, settings.MinimumSpeed, settings.MaximumSpeed);
            }

            fish.Speed = ClampSpeed(fish.Speed, settings);
            fish.Direction = CalculateDirection(
                fish.Direction,
                fish.Position,
                fish.Destination,
                fish.Speed,
                settings.TurningRadius,
                deltaTime);
            fish.Position = CalculatePosition(
                fish.Position,
                fish.Direction,
                fish.Speed,
                deltaTime,
                safeBounds);
        }

        public static Vector3 GenerateDestination(
            AquariumBounds safeBounds,
            Vector3 position,
            Vector3 currentDirection,
            float edgeDetectionRadius,
            Random random)
        {
            Vector3 forward = currentDirection.sqrMagnitude > 0f
                ? currentDirection.normalized
                : Vector3.forward;

            if (IsNearBoundary(position, safeBounds, edgeDetectionRadius))
            {
                Vector3 inward = safeBounds.Center - position;
                if (inward.sqrMagnitude > 0f)
                {
                    forward = Vector3.Slerp(forward, inward.normalized, 0.75f).normalized;
                }
            }

            float yaw = NextRange(random, -MaximumHeadingVariation, MaximumHeadingVariation);
            float pitch = NextRange(random, -MaximumVerticalVariation, MaximumVerticalVariation);
            Vector3 variedDirection = Quaternion.Euler(pitch, yaw, 0f) * forward;
            float travelDistance = NextRange(
                random,
                edgeDetectionRadius * 1.5f,
                edgeDetectionRadius * 3.5f);

            Vector3 destination = safeBounds.Clamp(position + variedDirection * travelDistance);
            if (Vector3.SqrMagnitude(destination - position) < MinimumDeltaTime)
            {
                destination = safeBounds.Center;
            }

            return destination;
        }

        public static Vector3 CalculateDirection(
            Vector3 currentDirection,
            Vector3 position,
            Vector3 destination,
            float speed,
            float turningRadius,
            float deltaTime)
        {
            Vector3 desiredDirection = destination - position;
            if (desiredDirection.sqrMagnitude < MinimumDeltaTime)
            {
                return currentDirection.sqrMagnitude > 0f
                    ? currentDirection.normalized
                    : Vector3.forward;
            }

            Vector3 current = currentDirection.sqrMagnitude > 0f
                ? currentDirection.normalized
                : desiredDirection.normalized;
            float angularSpeed = Mathf.Max(0.01f, speed) / Mathf.Max(0.01f, turningRadius);
            return Vector3.RotateTowards(
                current,
                desiredDirection.normalized,
                angularSpeed * Mathf.Max(0f, deltaTime),
                0f).normalized;
        }

        public static Vector3 CalculatePosition(
            Vector3 position,
            Vector3 direction,
            float speed,
            float deltaTime,
            AquariumBounds safeBounds)
        {
            if (!float.IsFinite(deltaTime) || deltaTime <= 0f)
            {
                return safeBounds.Clamp(position);
            }

            Vector3 normalizedDirection = direction.sqrMagnitude > 0f
                ? direction.normalized
                : Vector3.forward;
            Vector3 nextPosition = position + normalizedDirection * Mathf.Max(0f, speed) * deltaTime;
            return safeBounds.Clamp(nextPosition);
        }

        public static float ClampSpeed(float speed, FishMovementSettings settings)
        {
            if (!float.IsFinite(speed))
            {
                return settings.MinimumSpeed;
            }

            return Mathf.Clamp(speed, settings.MinimumSpeed, settings.MaximumSpeed);
        }

        public static AquariumBounds CreateSafeBounds(AquariumBounds bounds, float margin)
        {
            float safeMargin = Mathf.Max(0f, margin);
            Vector3 safeSize = bounds.Size - Vector3.one * (safeMargin * 2f);
            safeSize = new Vector3(
                Mathf.Max(0.001f, safeSize.x),
                Mathf.Max(0.001f, safeSize.y),
                Mathf.Max(0.001f, safeSize.z));
            return new AquariumBounds(bounds.Center, safeSize);
        }

        private static bool IsNearBoundary(
            Vector3 position,
            AquariumBounds bounds,
            float detectionRadius)
        {
            Vector3 minDistance = position - bounds.Min;
            Vector3 maxDistance = bounds.Max - position;
            float nearestMinimum = Mathf.Min(minDistance.x, Mathf.Min(minDistance.y, minDistance.z));
            float nearestMaximum = Mathf.Min(maxDistance.x, Mathf.Min(maxDistance.y, maxDistance.z));
            float nearestBoundary = Mathf.Min(nearestMinimum, nearestMaximum);
            return nearestBoundary <= Mathf.Max(0f, detectionRadius);
        }

        private static bool DestinationLeadsInward(
            Vector3 position,
            Vector3 destination,
            Vector3 center)
        {
            Vector3 destinationDirection = destination - position;
            Vector3 inwardDirection = center - position;
            if (destinationDirection.sqrMagnitude < MinimumDeltaTime
                || inwardDirection.sqrMagnitude < MinimumDeltaTime)
            {
                return false;
            }

            return Vector3.Dot(destinationDirection.normalized, inwardDirection.normalized) > 0.25f;
        }

        private static float NextRange(Random random, float minimum, float maximum)
        {
            return Mathf.Lerp(minimum, maximum, (float)random.NextDouble());
        }
    }
}
