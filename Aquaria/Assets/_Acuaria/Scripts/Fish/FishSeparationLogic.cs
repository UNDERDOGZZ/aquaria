using UnityEngine;

namespace Acuaria.Fish
{
    public static class FishSeparationLogic
    {
        public static Vector3 Contribution(
            Vector3 selfPosition, Vector3 neighborPosition, float radius, float strength)
        {
            Vector3 offset = selfPosition - neighborPosition;
            float distanceSquared = offset.sqrMagnitude;
            float radiusSquared = radius * radius;
            if (distanceSquared <= 0.000001f || distanceSquared >= radiusSquared)
            {
                return Vector3.zero;
            }

            float distance = Mathf.Sqrt(distanceSquared);
            float weight = 1f - distance / Mathf.Max(0.01f, radius);
            return offset / distance * (weight * Mathf.Max(0f, strength));
        }

        public static Vector3 Limit(Vector3 force, float maximum)
        {
            if (!float.IsFinite(force.x) || !float.IsFinite(force.y) || !float.IsFinite(force.z))
            {
                return Vector3.zero;
            }

            return Vector3.ClampMagnitude(force, Mathf.Max(0f, maximum));
        }
    }
}
