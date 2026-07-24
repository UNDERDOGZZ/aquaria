using UnityEngine;

namespace Acuaria.Environment
{
    public static class CameraMotionMath
    {
        public static float ClampDistance(float distance, AquariumCameraConfig config)
        {
            if (!float.IsFinite(distance))
            {
                return (config.MinimumDistance + config.MaximumDistance) * 0.5f;
            }

            return Mathf.Clamp(distance, config.MinimumDistance, config.MaximumDistance);
        }

        public static Vector2 ClampAngles(Vector2 angles, AquariumCameraConfig config)
        {
            if (!IsFinite(angles))
            {
                return Vector2.zero;
            }

            return new Vector2(
                Mathf.Clamp(angles.x, config.MinimumVerticalAngle, config.MaximumVerticalAngle),
                Mathf.Clamp(angles.y, config.MinimumHorizontalAngle, config.MaximumHorizontalAngle));
        }

        public static Vector2 ClampPan(Vector2 pan, Vector2 limits)
        {
            if (!IsFinite(pan))
            {
                return Vector2.zero;
            }

            return new Vector2(
                Mathf.Clamp(pan.x, -Mathf.Abs(limits.x), Mathf.Abs(limits.x)),
                Mathf.Clamp(pan.y, -Mathf.Abs(limits.y), Mathf.Abs(limits.y)));
        }

        private static bool IsFinite(Vector2 value)
        {
            return float.IsFinite(value.x) && float.IsFinite(value.y);
        }
    }
}
