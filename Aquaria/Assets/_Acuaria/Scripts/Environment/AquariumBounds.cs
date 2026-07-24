using System;
using UnityEngine;

namespace Acuaria.Environment
{
    [Serializable]
    public struct AquariumBounds
    {
        private const float MinimumSize = 0.001f;

        [SerializeField] private Vector3 center;
        [SerializeField] private Vector3 size;

        public AquariumBounds(Vector3 center, Vector3 size)
        {
            this.center = center;
            this.size = SanitizeSize(size);
        }

        public Vector3 Center => center;
        public Vector3 Size => SanitizeSize(size);
        public Vector3 Extents => Size * 0.5f;
        public Vector3 Min => center - Extents;
        public Vector3 Max => center + Extents;
        public bool IsValid => IsFinite(center) && IsFinite(size)
            && size.x > 0f && size.y > 0f && size.z > 0f;

        public bool Contains(Vector3 point)
        {
            if (!IsFinite(point))
            {
                return false;
            }

            Vector3 min = Min;
            Vector3 max = Max;
            return point.x >= min.x && point.x <= max.x
                && point.y >= min.y && point.y <= max.y
                && point.z >= min.z && point.z <= max.z;
        }

        public Vector3 Clamp(Vector3 point)
        {
            if (!IsFinite(point))
            {
                return center;
            }

            Vector3 min = Min;
            Vector3 max = Max;
            return new Vector3(
                Mathf.Clamp(point.x, min.x, max.x),
                Mathf.Clamp(point.y, min.y, max.y),
                Mathf.Clamp(point.z, min.z, max.z));
        }

        private static Vector3 SanitizeSize(Vector3 value)
        {
            return new Vector3(
                SanitizeDimension(value.x),
                SanitizeDimension(value.y),
                SanitizeDimension(value.z));
        }

        private static float SanitizeDimension(float value)
        {
            return float.IsFinite(value) ? Mathf.Max(Mathf.Abs(value), MinimumSize) : MinimumSize;
        }

        private static bool IsFinite(Vector3 value)
        {
            return float.IsFinite(value.x) && float.IsFinite(value.y) && float.IsFinite(value.z);
        }
    }
}
