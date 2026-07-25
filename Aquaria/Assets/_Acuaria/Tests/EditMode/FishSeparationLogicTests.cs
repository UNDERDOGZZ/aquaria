using Acuaria.Fish;
using NUnit.Framework;
using UnityEngine;

namespace Acuaria.Tests
{
    public sealed class FishSeparationLogicTests
    {
        [Test]
        public void CloseNeighbor_GeneratesSeparation()
        {
            Vector3 force = FishSeparationLogic.Contribution(
                Vector3.zero, new Vector3(0.1f, 0f, 0f), 1f, 0.5f);
            Assert.That(force.x, Is.LessThan(0f));
        }

        [Test]
        public void DistantNeighbor_GeneratesNoSeparation()
        {
            Assert.That(FishSeparationLogic.Contribution(
                Vector3.zero, Vector3.right * 2f, 1f, 1f), Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void Force_IsFiniteAndLimited()
        {
            Vector3 force = FishSeparationLogic.Limit(new Vector3(10f, 3f, -2f), 0.4f);
            Assert.That(force.magnitude, Is.LessThanOrEqualTo(0.4001f));
            Assert.That(float.IsFinite(force.x) && float.IsFinite(force.y) && float.IsFinite(force.z), Is.True);
        }
    }
}
