using System;
using Acuaria.Environment;
using Acuaria.Fish;
using NUnit.Framework;
using UnityEngine;
using FishState = Acuaria.Fish.Fish;
using Random = System.Random;

namespace Acuaria.Tests
{
    public sealed class FishMovementLogicTests
    {
        private static readonly AquariumBounds Bounds =
            new AquariumBounds(new Vector3(0f, 1.6f, 0f), new Vector3(8f, 3.2f, 3f));

        private static readonly FishMovementSettings Settings =
            new FishMovementSettings(0.55f, 0.9f, 1.8f, 4.2f, 0.8f, 0.65f);

        [Test]
        public void GenerateDestination_ReturnsPointInsideSafeVolume()
        {
            AquariumBounds safeBounds = FishMovementLogic.CreateSafeBounds(
                Bounds,
                Settings.BodyMargin);

            Vector3 destination = FishMovementLogic.GenerateDestination(
                safeBounds,
                safeBounds.Center,
                Vector3.forward,
                Settings.EdgeDetectionRadius,
                new Random(7));

            Assert.That(safeBounds.Contains(destination), Is.True);
            Assert.That(destination, Is.Not.EqualTo(safeBounds.Center));
        }

        [Test]
        public void GenerateDestination_NearWallBiasesDirectionInward()
        {
            AquariumBounds safeBounds = FishMovementLogic.CreateSafeBounds(
                Bounds,
                Settings.BodyMargin);
            Vector3 position = new Vector3(
                safeBounds.Max.x - 0.05f,
                safeBounds.Center.y,
                safeBounds.Center.z);

            Vector3 destination = FishMovementLogic.GenerateDestination(
                safeBounds,
                position,
                Vector3.right,
                Settings.EdgeDetectionRadius,
                new Random(11));

            Vector3 inward = safeBounds.Center - position;
            Assert.That(Vector3.Dot(destination - position, inward), Is.GreaterThan(0f));
        }

        [Test]
        public void CalculatePosition_NeverLeavesVolume()
        {
            AquariumBounds safeBounds = FishMovementLogic.CreateSafeBounds(
                Bounds,
                Settings.BodyMargin);

            Vector3 position = FishMovementLogic.CalculatePosition(
                safeBounds.Max,
                Vector3.one,
                Settings.MaximumSpeed,
                10f,
                safeBounds);

            Assert.That(safeBounds.Contains(position), Is.True);
        }

        [Test]
        public void CalculateDirection_TurnsTowardDestinationWithoutInstantRotation()
        {
            Vector3 direction = FishMovementLogic.CalculateDirection(
                Vector3.forward,
                Vector3.zero,
                Vector3.right * 5f,
                0.7f,
                0.8f,
                0.1f);

            Assert.That(Vector3.Dot(direction, Vector3.right), Is.GreaterThan(0f));
            Assert.That(direction, Is.Not.EqualTo(Vector3.right));
            Assert.That(direction.magnitude, Is.EqualTo(1f).Within(0.0001f));
        }

        [TestCase(-10f, 0.55f)]
        [TestCase(0.7f, 0.7f)]
        [TestCase(10f, 0.9f)]
        public void ClampSpeed_ReturnsValidSpeciesSpeed(float input, float expected)
        {
            Assert.That(FishMovementLogic.ClampSpeed(input, Settings), Is.EqualTo(expected));
        }

        [Test]
        public void Advance_KeepsFishInsideOverLongSimulation()
        {
            FishState fish = new FishState(
                Bounds.Center,
                Vector3.right,
                0.7f,
                Bounds.Max,
                0f);
            Random random = new Random(19);

            for (int i = 0; i < 3600; i++)
            {
                FishMovementLogic.Advance(fish, Bounds, Settings, 1f / 60f, random);
            }

            AquariumBounds safeBounds = FishMovementLogic.CreateSafeBounds(
                Bounds,
                Settings.BodyMargin);
            Assert.That(safeBounds.Contains(fish.Position), Is.True);
            Assert.That(fish.Speed, Is.InRange(Settings.MinimumSpeed, Settings.MaximumSpeed));
        }
    }
}
