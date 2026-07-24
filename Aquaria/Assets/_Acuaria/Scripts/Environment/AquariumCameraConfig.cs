using System;
using UnityEngine;

namespace Acuaria.Environment
{
    [Serializable]
    public sealed class AquariumCameraConfig
    {
        [Header("Input")]
        [SerializeField, Min(0f)] private float horizontalSpeed = 0.12f;
        [SerializeField, Min(0f)] private float verticalSpeed = 0.08f;
        [SerializeField, Min(0f)] private float zoomSensitivity = 0.012f;
        [SerializeField, Min(0f)] private float panSensitivity = 0.003f;

        [Header("Smoothing")]
        [SerializeField, Min(0.01f)] private float smoothingTime = 0.16f;

        [Header("Distance")]
        [SerializeField, Min(0.1f)] private float minimumDistance = 7f;
        [SerializeField, Min(0.1f)] private float maximumDistance = 13f;

        [Header("Angles")]
        [SerializeField] private float minimumHorizontalAngle = -35f;
        [SerializeField] private float maximumHorizontalAngle = 35f;
        [SerializeField] private float minimumVerticalAngle = -8f;
        [SerializeField] private float maximumVerticalAngle = 18f;

        [Header("Pan")]
        [SerializeField] private Vector2 panLimits = new Vector2(1.2f, 0.65f);

        [Header("Initial View")]
        [SerializeField] private Vector3 initialPosition = new Vector3(0f, 1.6f, 0f);
        [SerializeField] private Vector2 initialRotation = new Vector2(6f, 0f);
        [SerializeField, Min(0.1f)] private float initialDistance = 10f;

        public float HorizontalSpeed => horizontalSpeed;
        public float VerticalSpeed => verticalSpeed;
        public float ZoomSensitivity => zoomSensitivity;
        public float PanSensitivity => panSensitivity;
        public float SmoothingTime => Mathf.Max(0.01f, smoothingTime);
        public float MinimumDistance => Mathf.Min(minimumDistance, maximumDistance);
        public float MaximumDistance => Mathf.Max(minimumDistance, maximumDistance);
        public float MinimumHorizontalAngle => Mathf.Min(minimumHorizontalAngle, maximumHorizontalAngle);
        public float MaximumHorizontalAngle => Mathf.Max(minimumHorizontalAngle, maximumHorizontalAngle);
        public float MinimumVerticalAngle => Mathf.Min(minimumVerticalAngle, maximumVerticalAngle);
        public float MaximumVerticalAngle => Mathf.Max(minimumVerticalAngle, maximumVerticalAngle);
        public Vector2 PanLimits => new Vector2(Mathf.Abs(panLimits.x), Mathf.Abs(panLimits.y));
        public Vector3 InitialPosition => initialPosition;
        public Vector2 InitialRotation => CameraMotionMath.ClampAngles(initialRotation, this);
        public float InitialDistance => CameraMotionMath.ClampDistance(initialDistance, this);
    }
}
