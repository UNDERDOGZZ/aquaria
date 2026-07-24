using UnityEngine;

namespace Acuaria.Input
{
    public readonly struct CameraInputState
    {
        public CameraInputState(Vector2 orbitDelta, Vector2 panDelta, float zoomDelta)
        {
            OrbitDelta = IsFinite(orbitDelta) ? orbitDelta : Vector2.zero;
            PanDelta = IsFinite(panDelta) ? panDelta : Vector2.zero;
            ZoomDelta = float.IsFinite(zoomDelta) ? zoomDelta : 0f;
        }

        public Vector2 OrbitDelta { get; }
        public Vector2 PanDelta { get; }
        public float ZoomDelta { get; }

        private static bool IsFinite(Vector2 value)
        {
            return float.IsFinite(value.x) && float.IsFinite(value.y);
        }
    }
}
