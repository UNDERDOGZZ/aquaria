using System.Collections.Generic;
using UnityEngine;

namespace Acuaria.Fish
{
    [DisallowMultipleComponent]
    public sealed class FishRegistry : MonoBehaviour
    {
        private readonly List<FishMovement> _activeFish = new List<FishMovement>(24);
        public IReadOnlyList<FishMovement> ActiveFish => _activeFish;

        public void Register(FishMovement fish)
        {
            if (fish != null && !_activeFish.Contains(fish))
            {
                _activeFish.Add(fish);
            }
        }

        public void Unregister(FishMovement fish)
        {
            _activeFish.Remove(fish);
        }
    }
}
