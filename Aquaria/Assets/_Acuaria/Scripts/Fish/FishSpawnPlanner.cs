using System.Collections.Generic;
using Acuaria.Environment;
using UnityEngine;
using Random = System.Random;

namespace Acuaria.Fish
{
    public static class FishSpawnPlanner
    {
        private const int PositionAttempts = 16;

        public static List<FishSpawnPlan> CreatePlans(
            IReadOnlyList<FishSpawnGroup> groups, AquariumBounds bounds)
        {
            List<FishSpawnPlan> plans = new List<FishSpawnPlan>(20);
            if (groups == null) return plans;
            for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
            {
                FishSpawnGroup group = groups[groupIndex];
                if (group == null || group.Species == null) continue;
                Random random = new Random(group.Seed);
                FishMovementSettings settings = group.Species.MovementSettings;
                AquariumBounds safe = FishMovementLogic.CreateSafeBounds(bounds,
                    Mathf.Max(settings.WallSafetyDistance, settings.BodyMargin));
                for (int index = 0; index < group.Count; index++)
                {
                    Vector3 position = FindPosition(random, safe, settings, plans,
                        group.MinimumInitialSeparation);
                    float heading = Range(random, 0f, 360f);
                    Vector3 direction = Quaternion.Euler(0f, heading, 0f) * Vector3.forward;
                    float scale = Range(random, settings.MinimumScale, settings.MaximumScale);
                    float speed = Range(random, settings.MinimumSpeed, settings.MaximumSpeed);
                    int seed = random.Next(1, int.MaxValue);
                    string id = $"{group.Species.SpeciesId}-{groupIndex:D2}-{index:D3}-{seed:X8}";
                    plans.Add(new FishSpawnPlan(group.Species, position, direction,
                        scale, speed, seed, id));
                }
            }
            return plans;
        }

        private static Vector3 FindPosition(Random random, AquariumBounds safe,
            FishMovementSettings settings, List<FishSpawnPlan> existing, float separation)
        {
            float minY = Mathf.Lerp(safe.Min.y, safe.Max.y, settings.PreferredDepthMinimum);
            float maxY = Mathf.Lerp(safe.Min.y, safe.Max.y, settings.PreferredDepthMaximum);
            Vector3 candidate = safe.Center;
            for (int attempt = 0; attempt < PositionAttempts; attempt++)
            {
                candidate = new Vector3(Range(random, safe.Min.x, safe.Max.x),
                    Range(random, minY, maxY), Range(random, safe.Min.z, safe.Max.z));
                bool valid = true;
                for (int i = 0; i < existing.Count; i++)
                {
                    if ((existing[i].Position - candidate).sqrMagnitude < separation * separation)
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid) return candidate;
            }
            return safe.Clamp(candidate);
        }

        private static float Range(Random random, float min, float max)
        {
            return Mathf.Lerp(min, max, (float)random.NextDouble());
        }
    }
}
