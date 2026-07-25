using System.Collections.Generic;
using Acuaria.Environment;
using Acuaria.Fish;
using NUnit.Framework;
using UnityEngine;

namespace Acuaria.Tests
{
    public sealed class FishSpawnPlannerTests
    {
        [Test]
        public void Plans_HonorCountsBoundsScalesSpeedsSpeciesAndSeeds()
        {
            FishSpecies first = ScriptableObject.CreateInstance<FishSpecies>();
            FishSpecies second = ScriptableObject.CreateInstance<FishSpecies>();
            var groups = new List<FishSpawnGroup>
            {
                new FishSpawnGroup(first, 3, 0.2f, 10),
                new FishSpawnGroup(second, 2, 0.2f, 20)
            };
            AquariumBounds bounds = new AquariumBounds(Vector3.zero, new Vector3(8f, 3f, 3f));
            List<FishSpawnPlan> plans = FishSpawnPlanner.CreatePlans(groups, bounds);

            Assert.That(plans, Has.Count.EqualTo(5));
            HashSet<int> seeds = new HashSet<int>();
            for (int i = 0; i < plans.Count; i++)
            {
                FishSpawnPlan plan = plans[i];
                Assert.That(bounds.Contains(plan.Position), Is.True);
                FishMovementSettings settings = plan.Species.MovementSettings;
                Assert.That(plan.Scale, Is.InRange(settings.MinimumScale, settings.MaximumScale));
                Assert.That(plan.Speed, Is.InRange(settings.MinimumSpeed, settings.MaximumSpeed));
                Assert.That(seeds.Add(plan.Seed), Is.True);
                Assert.That(string.IsNullOrWhiteSpace(plan.InstanceId), Is.False);
            }
            Assert.That(plans[0].Species, Is.SameAs(first));
            Assert.That(plans[3].Species, Is.SameAs(second));
            Object.DestroyImmediate(first);
            Object.DestroyImmediate(second);
        }

        [Test]
        public void SameSeeds_CreateDeterministicPlans()
        {
            FishSpecies species = ScriptableObject.CreateInstance<FishSpecies>();
            var groups = new[] { new FishSpawnGroup(species, 2, 0.2f, 77) };
            AquariumBounds bounds = new AquariumBounds(Vector3.zero, new Vector3(8f, 3f, 3f));
            List<FishSpawnPlan> first = FishSpawnPlanner.CreatePlans(groups, bounds);
            List<FishSpawnPlan> second = FishSpawnPlanner.CreatePlans(groups, bounds);
            Assert.That(second[0].Position, Is.EqualTo(first[0].Position));
            Assert.That(second[0].Direction, Is.EqualTo(first[0].Direction));
            Object.DestroyImmediate(species);
        }
    }
}
