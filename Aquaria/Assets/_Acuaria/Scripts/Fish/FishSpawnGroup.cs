using System;
using UnityEngine;

namespace Acuaria.Fish
{
    [Serializable]
    public sealed class FishSpawnGroup
    {
        [SerializeField] private FishSpecies species;
        [SerializeField, Min(1)] private int count = 1;
        [SerializeField, Min(0f)] private float minimumInitialSeparation = 0.4f;
        [SerializeField] private int seed = 1001;

        public FishSpawnGroup()
        {
        }

        public FishSpawnGroup(FishSpecies species, int count, float minimumSeparation, int seed)
        {
            this.species = species;
            this.count = Mathf.Max(1, count);
            minimumInitialSeparation = Mathf.Max(0f, minimumSeparation);
            this.seed = seed;
        }

        public FishSpecies Species => species;
        public int Count => Mathf.Max(1, count);
        public float MinimumInitialSeparation => Mathf.Max(0f, minimumInitialSeparation);
        public int Seed => seed;
    }
}
