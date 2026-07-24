using Acuaria.Environment;
using NUnit.Framework;
using UnityEngine;

namespace Acuaria.Tests
{
    public sealed class AquariumBoundsTests
    {
        [Test]
        public void Contains_ReturnsTrue_ForPointInsideVolume()
        {
            AquariumBounds bounds = new AquariumBounds(Vector3.zero, new Vector3(8f, 4f, 3f));

            Assert.That(bounds.Contains(new Vector3(3.9f, 1.9f, 1.4f)), Is.True);
        }

        [Test]
        public void Contains_ReturnsFalse_ForPointOutsideVolume()
        {
            AquariumBounds bounds = new AquariumBounds(Vector3.zero, new Vector3(8f, 4f, 3f));

            Assert.That(bounds.Contains(new Vector3(4.1f, 0f, 0f)), Is.False);
        }

        [Test]
        public void Clamp_RestrictsPointToVolume()
        {
            AquariumBounds bounds = new AquariumBounds(Vector3.zero, new Vector3(8f, 4f, 3f));

            Assert.That(bounds.Clamp(new Vector3(10f, -10f, 2f)),
                Is.EqualTo(new Vector3(4f, -2f, 1.5f)));
        }

        [Test]
        public void Constructor_SanitizesInvalidSizes()
        {
            AquariumBounds bounds = new AquariumBounds(Vector3.zero, new Vector3(-8f, 0f, -3f));

            Assert.That(bounds.Size.x, Is.EqualTo(8f));
            Assert.That(bounds.Size.y, Is.GreaterThan(0f));
            Assert.That(bounds.Size.z, Is.EqualTo(3f));
        }

        [Test]
        public void Clamp_HandlesReasonableExtremeValues()
        {
            AquariumBounds bounds = new AquariumBounds(
                new Vector3(1000f, -1000f, 500f),
                new Vector3(200f, 100f, 50f));

            Assert.That(bounds.Clamp(new Vector3(float.MaxValue, float.MinValue, 500f)),
                Is.EqualTo(new Vector3(1100f, -1050f, 500f)));
        }
    }
}
