using System.Collections.Generic;
using Acuaria.Environment;
using UnityEngine;

namespace Acuaria.Fish
{
    [DisallowMultipleComponent]
    public sealed class FishSpawner : MonoBehaviour
    {
        [SerializeField] private AquariumVolume aquariumVolume;
        [SerializeField] private FishRegistry registry;
        [SerializeField] private Transform runtimeParent;
        [SerializeField] private FishSpawnGroup[] groups;

        private readonly List<FishMovement> _spawnedFish = new List<FishMovement>(20);
        public IReadOnlyList<FishMovement> SpawnedFish => _spawnedFish;

        public void Configure(FishSpecies species, FishMovement prefab, AquariumVolume volume)
        {
            aquariumVolume = volume;
        }

        public void Configure(AquariumVolume volume, FishRegistry fishRegistry,
            Transform parent, FishSpawnGroup[] spawnGroups)
        {
            aquariumVolume = volume;
            registry = fishRegistry;
            runtimeParent = parent;
            groups = spawnGroups;
        }

        private void Awake()
        {
            if (aquariumVolume == null || registry == null || runtimeParent == null || groups == null)
            {
                Debug.LogError($"{nameof(FishSpawner)} requires volume, registry, runtime parent and groups.", this);
                enabled = false;
                return;
            }
            SpawnConfiguredPopulation();
        }

        public void SpawnConfiguredPopulation()
        {
            if (_spawnedFish.Count > 0) return;
            List<FishSpawnPlan> plans = FishSpawnPlanner.CreatePlans(groups, aquariumVolume.WorldBounds);
            for (int i = 0; i < plans.Count; i++)
            {
                FishSpawnPlan plan = plans[i];
                FishMovement prefab = plan.Species.VisualPrefab;
                if (prefab == null)
                {
                    Debug.LogWarning($"Species {plan.Species.SpeciesId} has no visual prefab.", this);
                    continue;
                }
                FishMovement fish = Instantiate(prefab, runtimeParent);
                fish.name = $"{plan.Species.DisplayName} [{i + 1}]";
                fish.Initialize(plan.Species, aquariumVolume, registry, plan.Seed,
                    plan.Position, plan.Direction, plan.Scale, plan.Speed, plan.InstanceId);
                _spawnedFish.Add(fish);
            }
        }
    }
}
