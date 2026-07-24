using Acuaria.Environment;
using UnityEngine;

namespace Acuaria.Fish
{
    [DisallowMultipleComponent]
    public sealed class FishSpawner : MonoBehaviour
    {
        [SerializeField] private FishSpecies species;
        [SerializeField] private FishMovement fishPrefab;
        [SerializeField] private AquariumVolume aquariumVolume;
        [SerializeField] private int randomSeed = 7391;

        private FishMovement _spawnedFish;

        public void Configure(
            FishSpecies fishSpecies,
            FishMovement prototypePrefab,
            AquariumVolume volume)
        {
            species = fishSpecies;
            fishPrefab = prototypePrefab;
            aquariumVolume = volume;
        }

        private void Awake()
        {
            if (species == null || fishPrefab == null || aquariumVolume == null)
            {
                Debug.LogError($"{nameof(FishSpawner)} requires species, prefab and aquarium volume.", this);
                enabled = false;
                return;
            }

            Spawn();
        }

        public FishMovement Spawn()
        {
            if (_spawnedFish != null)
            {
                return _spawnedFish;
            }

            _spawnedFish = Instantiate(fishPrefab, transform);
            _spawnedFish.name = species.DisplayName;
            _spawnedFish.Initialize(species, aquariumVolume, randomSeed);
            return _spawnedFish;
        }
    }
}
