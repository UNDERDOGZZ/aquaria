using System.Collections.Generic;
using Acuaria.Environment;
using UnityEngine;
using Random = System.Random;

namespace Acuaria.Fish
{
    [DisallowMultipleComponent]
    public sealed class FishMovement : MonoBehaviour
    {
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Renderer[] colorRenderers;
        [SerializeField] private bool showDebugGizmos;

        private MaterialPropertyBlock _propertyBlock;
        private FishSpecies _species;
        private AquariumVolume _aquariumVolume;
        private FishRegistry _registry;
        private Fish _fish;
        private Random _random;
        private float _animationPhase;

        public Fish State => _fish;

        public void ConfigureVisual(Transform root, Renderer[] renderers)
        {
            visualRoot = root;
            colorRenderers = renderers;
        }

        public void Initialize(FishSpecies species, AquariumVolume volume, int seed)
        {
            Initialize(species, volume, null, seed, volume.WorldBounds.Center,
                Vector3.right, species.MovementSettings.Size,
                (species.MovementSettings.MinimumSpeed + species.MovementSettings.MaximumSpeed) * 0.5f,
                $"{species.SpeciesId}-{seed}");
        }

        public void Initialize(FishSpecies species, AquariumVolume volume, FishRegistry registry,
            int seed, Vector3 position, Vector3 direction, float scale, float speed, string instanceId)
        {
            _species = species;
            _aquariumVolume = volume;
            _registry = registry;
            _random = new Random(seed);
            _animationPhase = (float)_random.NextDouble() * Mathf.PI * 2f;
            FishMovementSettings settings = species.MovementSettings;
            AquariumBounds safe = FishMovementLogic.CreateSafeBounds(volume.WorldBounds,
                Mathf.Max(settings.WallSafetyDistance, scale * 0.5f));
            Vector3 destination = FishMovementLogic.GenerateDestination(
                safe, position, direction, settings, _random);
            _fish = new Fish(instanceId, species, safe.Clamp(position), direction,
                speed, speed, destination, scale, seed, 0f);
            transform.localScale = Vector3.one * scale;
            ApplyColor(species.PrototypeColor);
            _registry?.Register(this);
            ApplyState(0f);
        }

        private void OnDisable()
        {
            _registry?.Unregister(this);
        }

        private void Update()
        {
            if (_fish == null || _species == null || _aquariumVolume == null)
            {
                return;
            }
            Vector3 separation = CalculateSeparation();
            FishMovementLogic.Advance(_fish, _aquariumVolume.WorldBounds,
                _species.MovementSettings, Time.deltaTime, _random, separation);
            ApplyState(Time.deltaTime);
        }

        private Vector3 CalculateSeparation()
        {
            if (_registry == null)
            {
                return Vector3.zero;
            }
            FishMovementSettings settings = _species.MovementSettings;
            Vector3 force = Vector3.zero;
            IReadOnlyList<FishMovement> fish = _registry.ActiveFish;
            for (int i = 0; i < fish.Count; i++)
            {
                FishMovement neighbor = fish[i];
                if (neighbor == this || neighbor == null || neighbor.State == null)
                {
                    continue;
                }
                force += FishSeparationLogic.Contribution(_fish.Position,
                    neighbor.State.Position, settings.SeparationRadius, settings.SeparationStrength);
            }
            return FishSeparationLogic.Limit(force, settings.MaximumSeparation);
        }

        private void ApplyState(float deltaTime)
        {
            transform.position = _fish.Position;
            FishMovementSettings settings = _species.MovementSettings;
            Quaternion target = FishOrientationLogic.CreateStableRotation(
                _fish.SmoothedDirection, settings.MaximumAscentAngle, settings.MaximumDescentAngle);
            float signedTurn = Vector3.SignedAngle(
                transform.forward, target * Vector3.forward, Vector3.up);
            float targetBank = Mathf.Clamp(
                -signedTurn * 0.12f, -settings.MaximumVisualBank, settings.MaximumVisualBank);
            _fish.VisualBank = Mathf.Lerp(
                _fish.VisualBank, targetBank, 1f - Mathf.Exp(-4f * Mathf.Max(deltaTime, 0.0001f)));
            transform.rotation = Quaternion.Slerp(transform.rotation, target,
                1f - Mathf.Exp(-settings.TurningSpeed * Mathf.Deg2Rad * Mathf.Max(deltaTime, 0.0001f)));
            if (visualRoot != null)
            {
                float oscillation = Mathf.Sin(Time.time * settings.SwimOscillation * Mathf.PI * 2f
                    + _animationPhase) * 2f;
                visualRoot.localRotation = Quaternion.Euler(0f, oscillation, _fish.VisualBank);
            }
        }

        private void ApplyColor(Color color)
        {
            if (_propertyBlock == null)
            {
                _propertyBlock = new MaterialPropertyBlock();
            }
            _propertyBlock.SetColor("_BaseColor", color);
            if (colorRenderers == null) return;
            for (int i = 0; i < colorRenderers.Length; i++)
            {
                colorRenderers[i]?.SetPropertyBlock(_propertyBlock);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!showDebugGizmos || _fish == null || _species == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_fish.Position, _fish.Position + _fish.Direction);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_fish.Position, _fish.Position + _fish.SmoothedDirection);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_fish.Destination, 0.08f);
            Gizmos.DrawLine(_fish.Position, _fish.Destination);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_fish.Position, _species.MovementSettings.SeparationRadius);
        }
    }
}
