using System;
using Acuaria.Environment;
using UnityEngine;
using Random = System.Random;

namespace Acuaria.Fish
{
    [DisallowMultipleComponent]
    public sealed class FishMovement : MonoBehaviour
    {
        [SerializeField] private bool showDebugGizmos;

        private FishSpecies _species;
        private AquariumVolume _aquariumVolume;
        private Fish _fish;
        private Random _random;

        public Fish State => _fish;

        public void Initialize(FishSpecies species, AquariumVolume aquariumVolume, int randomSeed)
        {
            _species = species;
            _aquariumVolume = aquariumVolume;
            _random = new Random(randomSeed);

            FishMovementSettings settings = species.MovementSettings;
            AquariumBounds safeBounds = FishMovementLogic.CreateSafeBounds(
                aquariumVolume.WorldBounds,
                settings.BodyMargin);
            Vector3 initialDirection = Vector3.right;
            Vector3 initialPosition = safeBounds.Center;
            Vector3 initialDestination = FishMovementLogic.GenerateDestination(
                safeBounds,
                initialPosition,
                initialDirection,
                settings.EdgeDetectionRadius,
                _random);

            _fish = new Fish(
                initialPosition,
                initialDirection,
                (settings.MinimumSpeed + settings.MaximumSpeed) * 0.5f,
                initialDestination,
                0f);
            transform.localScale = Vector3.one * settings.Size;
            ApplyState();
        }

        private void Update()
        {
            if (_fish == null || _species == null || _aquariumVolume == null)
            {
                return;
            }

            FishMovementLogic.Advance(
                _fish,
                _aquariumVolume.WorldBounds,
                _species.MovementSettings,
                Time.deltaTime,
                _random);
            ApplyState();
        }

        private void ApplyState()
        {
            transform.position = _fish.Position;
            if (_fish.Direction.sqrMagnitude > 0f)
            {
                transform.rotation = Quaternion.LookRotation(_fish.Direction, Vector3.up);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!showDebugGizmos || _fish == null || _species == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_fish.Position, _fish.Position + _fish.Direction);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_fish.Destination, 0.08f);
            Gizmos.DrawLine(_fish.Position, _fish.Destination);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_fish.Position, _species.MovementSettings.EdgeDetectionRadius);
        }
    }
}
