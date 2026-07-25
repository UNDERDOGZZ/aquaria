using Acuaria.Fish;
using NUnit.Framework;
using UnityEngine;

namespace Acuaria.Tests
{
    public sealed class FishSpeciesSettingsTests
    {
        [Test]
        public void Settings_SanitizeRangesAndAngles()
        {
            FishMovementSettings settings = new FishMovementSettings(
                2f, -1f, 3f, -2f, -1f, -1f, -30f, 5f, 1f,
                -2f, 0.9f, 0.1f, -1f, 90f, -20f, -1f, -1f, -1f, 99f, -2f);
            Assert.That(settings.MinimumScale, Is.GreaterThan(0f));
            Assert.That(settings.MaximumScale, Is.GreaterThanOrEqualTo(settings.MinimumScale));
            Assert.That(settings.MaximumSpeed, Is.GreaterThanOrEqualTo(settings.MinimumSpeed));
            Assert.That(settings.PreferredDepthMinimum, Is.LessThanOrEqualTo(settings.PreferredDepthMaximum));
            Assert.That(settings.MaximumAscentAngle, Is.InRange(0f, 45f));
            Assert.That(settings.MaximumDescentAngle, Is.InRange(0f, 45f));
        }

        [Test]
        public void DefaultSpecies_HasStableIdentifier()
        {
            FishSpecies species = ScriptableObject.CreateInstance<FishSpecies>();
            Assert.That(string.IsNullOrWhiteSpace(species.SpeciesId), Is.False);
            Object.DestroyImmediate(species);
        }
    }
}
