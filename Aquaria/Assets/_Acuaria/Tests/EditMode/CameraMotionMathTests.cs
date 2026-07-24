using Acuaria.Environment;
using NUnit.Framework;
using UnityEngine;

namespace Acuaria.Tests
{
    public sealed class CameraMotionMathTests
    {
        private AquariumCameraConfig _config;

        [SetUp]
        public void SetUp()
        {
            _config = new AquariumCameraConfig();
        }

        [TestCase(1f, 7f)]
        [TestCase(10f, 10f)]
        [TestCase(20f, 13f)]
        public void ClampDistance_UsesConfiguredLimits(float input, float expected)
        {
            Assert.That(CameraMotionMath.ClampDistance(input, _config), Is.EqualTo(expected));
        }

        [Test]
        public void ClampAngles_UsesConfiguredVerticalAndHorizontalLimits()
        {
            Vector2 result = CameraMotionMath.ClampAngles(new Vector2(90f, -90f), _config);

            Assert.That(result, Is.EqualTo(new Vector2(18f, -35f)));
        }

        [Test]
        public void ClampPan_UsesIndependentAxisLimits()
        {
            Vector2 result = CameraMotionMath.ClampPan(new Vector2(5f, -5f), new Vector2(1.2f, 0.65f));

            Assert.That(result, Is.EqualTo(new Vector2(1.2f, -0.65f)));
        }

        [Test]
        public void ClampPan_RejectsNonFiniteInput()
        {
            Vector2 result = CameraMotionMath.ClampPan(
                new Vector2(float.NaN, float.PositiveInfinity),
                new Vector2(1f, 1f));

            Assert.That(result, Is.EqualTo(Vector2.zero));
        }
    }
}
