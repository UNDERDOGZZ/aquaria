using Acuaria.Input;
using UnityEngine;

namespace Acuaria.Environment
{
    [DisallowMultipleComponent]
    public sealed class AquariumCameraController : MonoBehaviour
    {
        [SerializeField] private PointerInputReader inputReader;
        [SerializeField] private AquariumVolume aquariumVolume;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Camera controlledCamera;
        [SerializeField] private AquariumCameraConfig config = new AquariumCameraConfig();

        private Vector2 _targetAngles;
        private Vector2 _currentAngles;
        private Vector2 _angleVelocity;
        private Vector2 _targetPan;
        private Vector2 _currentPan;
        private Vector2 _panVelocity;
        private float _targetDistance;
        private float _currentDistance;
        private float _distanceVelocity;

        private void Awake()
        {
            ResetCamera();
        }

        private void LateUpdate()
        {
            if (inputReader == null || cameraPivot == null || controlledCamera == null)
            {
                return;
            }

            CameraInputState input = inputReader.CurrentState;
            _targetAngles += new Vector2(
                -input.OrbitDelta.y * config.VerticalSpeed,
                input.OrbitDelta.x * config.HorizontalSpeed);
            _targetAngles = CameraMotionMath.ClampAngles(_targetAngles, config);

            _targetDistance = CameraMotionMath.ClampDistance(
                _targetDistance - input.ZoomDelta * config.ZoomSensitivity,
                config);

            _targetPan += new Vector2(
                -input.PanDelta.x * config.PanSensitivity,
                -input.PanDelta.y * config.PanSensitivity);
            _targetPan = CameraMotionMath.ClampPan(_targetPan, config.PanLimits);

            float deltaTime = Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
            _currentAngles.x = Mathf.SmoothDampAngle(
                _currentAngles.x, _targetAngles.x, ref _angleVelocity.x, config.SmoothingTime,
                Mathf.Infinity, deltaTime);
            _currentAngles.y = Mathf.SmoothDampAngle(
                _currentAngles.y, _targetAngles.y, ref _angleVelocity.y, config.SmoothingTime,
                Mathf.Infinity, deltaTime);
            _currentDistance = Mathf.SmoothDamp(
                _currentDistance, _targetDistance, ref _distanceVelocity, config.SmoothingTime,
                Mathf.Infinity, deltaTime);
            _currentPan = Vector2.SmoothDamp(
                _currentPan, _targetPan, ref _panVelocity, config.SmoothingTime,
                Mathf.Infinity, deltaTime);

            ApplyView();
        }

        [ContextMenu("Reset Camera")]
        public void ResetCamera()
        {
            _targetAngles = config.InitialRotation;
            _currentAngles = _targetAngles;
            _angleVelocity = Vector2.zero;
            _targetPan = Vector2.zero;
            _currentPan = Vector2.zero;
            _panVelocity = Vector2.zero;
            _targetDistance = config.InitialDistance;
            _currentDistance = _targetDistance;
            _distanceVelocity = 0f;
            ApplyView();
        }

        private void ApplyView()
        {
            if (cameraPivot == null || controlledCamera == null)
            {
                return;
            }

            Vector3 center = aquariumVolume != null
                ? aquariumVolume.WorldBounds.Center
                : config.InitialPosition;
            cameraPivot.position = center + new Vector3(_currentPan.x, _currentPan.y, 0f);
            cameraPivot.rotation = Quaternion.Euler(_currentAngles.x, _currentAngles.y, 0f);
            controlledCamera.transform.localPosition = new Vector3(0f, 0f, -_currentDistance);
            controlledCamera.transform.localRotation = Quaternion.identity;
        }
    }
}
