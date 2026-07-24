using UnityEngine;

namespace Acuaria.Fish
{
    [CreateAssetMenu(fileName = "FishSpecies", menuName = "Acuaria/Fish/Fish Species")]
    public sealed class FishSpecies : ScriptableObject
    {
        [SerializeField] private string displayName = "Prototype Fish";
        [SerializeField, Min(0.01f)] private float minimumSpeed = 0.55f;
        [SerializeField, Min(0.01f)] private float maximumSpeed = 0.9f;
        [SerializeField, Min(0.1f)] private float minimumDirectionTime = 1.8f;
        [SerializeField, Min(0.1f)] private float maximumDirectionTime = 4.2f;
        [SerializeField, Min(0.01f)] private float turningRadius = 0.8f;
        [SerializeField, Min(0.01f)] private float size = 0.65f;
        [SerializeField] private Color prototypeColor = new Color(0.15f, 0.65f, 0.85f, 1f);

        public string DisplayName => displayName;
        public Color PrototypeColor => prototypeColor;

        public FishMovementSettings MovementSettings => new FishMovementSettings(
            minimumSpeed,
            maximumSpeed,
            minimumDirectionTime,
            maximumDirectionTime,
            turningRadius,
            size);
    }
}
