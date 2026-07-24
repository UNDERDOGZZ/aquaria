using UnityEngine;

namespace Acuaria.Environment
{
    [DisallowMultipleComponent]
    public sealed class AquariumVolume : MonoBehaviour
    {
        [SerializeField] private AquariumBounds localBounds =
            new AquariumBounds(new Vector3(0f, 1.6f, 0f), new Vector3(8f, 3.2f, 3f));

        public AquariumBounds WorldBounds
        {
            get
            {
                Vector3 worldCenter = transform.TransformPoint(localBounds.Center);
                Vector3 worldSize = Vector3.Scale(localBounds.Size, Abs(transform.lossyScale));
                return new AquariumBounds(worldCenter, worldSize);
            }
        }

        private static Vector3 Abs(Vector3 value)
        {
            return new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
        }

        private void OnDrawGizmosSelected()
        {
            AquariumBounds bounds = WorldBounds;
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.8f);
            Gizmos.DrawWireCube(bounds.Center, bounds.Size);
        }
    }
}
