using UnityEngine;

namespace Acuaria.Fish
{
    [CreateAssetMenu(fileName = "FishSpecies", menuName = "Acuaria/Fish/Fish Species")]
    public sealed class FishSpecies : ScriptableObject
    {
        [SerializeField] private string speciesId = "prototype-fish";
        [SerializeField] private string displayName = "Prototype Fish";
        [SerializeField] private FishMovement visualPrefab;
        [SerializeField, Min(0.01f)] private float minimumScale = 0.55f;
        [SerializeField, Min(0.01f)] private float maximumScale = 0.75f;
        [SerializeField, Min(0.01f)] private float minimumSpeed = 0.55f;
        [SerializeField, Min(0.01f)] private float maximumSpeed = 0.9f;
        [SerializeField, Min(0.01f)] private float acceleration = 0.35f;
        [SerializeField, Min(0.01f)] private float deceleration = 0.5f;
        [SerializeField, Min(1f)] private float turningSpeed = 75f;
        [SerializeField, Min(0.1f)] private float minimumDirectionTime = 1.8f;
        [SerializeField, Min(0.1f)] private float maximumDirectionTime = 4.2f;
        [SerializeField, Min(0.01f)] private float wallSafetyDistance = 0.55f;
        [SerializeField, Range(0f, 1f)] private float preferredDepthMinimum = 0.25f;
        [SerializeField, Range(0f, 1f)] private float preferredDepthMaximum = 0.75f;
        [SerializeField, Min(0.01f)] private float maximumVerticalVariation = 0.65f;
        [SerializeField, Range(0f, 45f)] private float maximumAscentAngle = 18f;
        [SerializeField, Range(0f, 45f)] private float maximumDescentAngle = 15f;
        [SerializeField, Min(0.01f)] private float separationRadius = 0.65f;
        [SerializeField, Min(0f)] private float separationStrength = 0.65f;
        [SerializeField, Min(0f)] private float maximumSeparation = 0.45f;
        [SerializeField, Range(0f, 15f)] private float maximumVisualBank = 5f;
        [SerializeField, Range(0f, 5f)] private float swimOscillation = 1.2f;
        [SerializeField] private Color prototypeColor = new Color(0.15f, 0.65f, 0.85f, 1f);
        [SerializeField, Min(1)] private int suggestedPrototypeCount = 1;

        public string SpeciesId => speciesId;
        public string DisplayName => displayName;
        public FishMovement VisualPrefab => visualPrefab;
        public Color PrototypeColor => prototypeColor;
        public int SuggestedPrototypeCount => suggestedPrototypeCount;

        public FishMovementSettings MovementSettings => new FishMovementSettings(
            minimumScale, maximumScale, minimumSpeed, maximumSpeed, acceleration, deceleration,
            turningSpeed, minimumDirectionTime, maximumDirectionTime, wallSafetyDistance,
            preferredDepthMinimum, preferredDepthMaximum, maximumVerticalVariation,
            maximumAscentAngle, maximumDescentAngle, separationRadius, separationStrength,
            maximumSeparation, maximumVisualBank, swimOscillation);

        private void OnValidate()
        {
            speciesId = string.IsNullOrWhiteSpace(speciesId)
                ? name.Trim().ToLowerInvariant().Replace(' ', '-')
                : speciesId.Trim().ToLowerInvariant().Replace(' ', '-');
            minimumScale = Mathf.Max(0.01f, minimumScale);
            maximumScale = Mathf.Max(minimumScale, maximumScale);
            minimumSpeed = Mathf.Max(0.01f, minimumSpeed);
            maximumSpeed = Mathf.Max(minimumSpeed, maximumSpeed);
            acceleration = Mathf.Max(0.01f, acceleration);
            deceleration = Mathf.Max(0.01f, deceleration);
            turningSpeed = Mathf.Max(1f, turningSpeed);
            minimumDirectionTime = Mathf.Max(0.1f, minimumDirectionTime);
            maximumDirectionTime = Mathf.Max(minimumDirectionTime, maximumDirectionTime);
            preferredDepthMinimum = Mathf.Clamp01(preferredDepthMinimum);
            preferredDepthMaximum = Mathf.Max(preferredDepthMinimum, Mathf.Clamp01(preferredDepthMaximum));
            maximumAscentAngle = Mathf.Clamp(maximumAscentAngle, 0f, 45f);
            maximumDescentAngle = Mathf.Clamp(maximumDescentAngle, 0f, 45f);
            suggestedPrototypeCount = Mathf.Max(1, suggestedPrototypeCount);
        }
    }
}
