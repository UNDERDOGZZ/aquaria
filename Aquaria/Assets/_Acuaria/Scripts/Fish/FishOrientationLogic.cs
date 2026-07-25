using UnityEngine;

namespace Acuaria.Fish
{
    public static class FishOrientationLogic
    {
        public static Vector3 LimitVerticalDirection(
            Vector3 direction, float maximumAscentAngle, float maximumDescentAngle)
        {
            if (!IsFinite(direction) || direction.sqrMagnitude < 0.000001f)
            {
                return Vector3.forward;
            }

            Vector2 horizontal = new Vector2(direction.x, direction.z);
            if (horizontal.sqrMagnitude < 0.000001f)
            {
                horizontal = Vector2.up;
            }

            float horizontalMagnitude = horizontal.magnitude;
            float maximumY = Mathf.Tan(maximumAscentAngle * Mathf.Deg2Rad) * horizontalMagnitude;
            float minimumY = -Mathf.Tan(maximumDescentAngle * Mathf.Deg2Rad) * horizontalMagnitude;
            return new Vector3(direction.x, Mathf.Clamp(direction.y, minimumY, maximumY), direction.z).normalized;
        }

        public static Quaternion CreateStableRotation(
            Vector3 direction, float maximumAscentAngle, float maximumDescentAngle)
        {
            Vector3 limited = LimitVerticalDirection(direction, maximumAscentAngle, maximumDescentAngle);
            Quaternion rotation = Quaternion.LookRotation(limited, Vector3.up);
            return IsFinite(rotation) ? Quaternion.Normalize(rotation) : Quaternion.identity;
        }

        public static bool IsFinite(Quaternion value)
        {
            return float.IsFinite(value.x) && float.IsFinite(value.y)
                && float.IsFinite(value.z) && float.IsFinite(value.w);
        }

        private static bool IsFinite(Vector3 value)
        {
            return float.IsFinite(value.x) && float.IsFinite(value.y) && float.IsFinite(value.z);
        }
    }
}
