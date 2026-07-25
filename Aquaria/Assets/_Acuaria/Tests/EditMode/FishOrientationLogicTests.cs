using Acuaria.Fish;
using NUnit.Framework;
using UnityEngine;

namespace Acuaria.Tests
{
    public sealed class FishOrientationLogicTests
    {
        [Test]
        public void NearlyVerticalDirection_IsLimited()
        {
            Vector3 result = FishOrientationLogic.LimitVerticalDirection(
                new Vector3(0.001f, 1f, 0.001f), 18f, 15f);
            float pitch = Mathf.Atan2(result.y, new Vector2(result.x, result.z).magnitude) * Mathf.Rad2Deg;
            Assert.That(pitch, Is.LessThanOrEqualTo(18.01f));
        }

        [Test]
        public void StableRotation_IsFiniteNormalizedAndUpright()
        {
            Quaternion rotation = FishOrientationLogic.CreateStableRotation(
                new Vector3(0.2f, -0.9f, 0.1f), 18f, 15f);
            Assert.That(FishOrientationLogic.IsFinite(rotation), Is.True);
            Assert.That(Mathf.Abs(1f - Quaternion.Dot(rotation, rotation)), Is.LessThan(0.0001f));
            Assert.That(Vector3.Dot(rotation * Vector3.up, Vector3.up), Is.GreaterThan(0.9f));
        }

        [Test]
        public void InvalidDirection_ReturnsValidRotation()
        {
            Quaternion rotation = FishOrientationLogic.CreateStableRotation(
                new Vector3(float.NaN, 0f, 0f), 18f, 15f);
            Assert.That(FishOrientationLogic.IsFinite(rotation), Is.True);
        }
    }
}
