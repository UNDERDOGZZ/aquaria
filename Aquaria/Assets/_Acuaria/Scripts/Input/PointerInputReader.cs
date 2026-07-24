using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Acuaria.Input
{
    [DisallowMultipleComponent]
    public sealed class PointerInputReader : MonoBehaviour
    {
        private readonly List<TouchControl> _activeTouches = new List<TouchControl>(2);
        private CameraInputState _currentState;
        private int _previousTouchCount;

        public CameraInputState CurrentState => _currentState;

        private void Update()
        {
            _currentState = ReadTouchState();

            if (_activeTouches.Count == 0)
            {
                _currentState = ReadMouseState();
            }
        }

        private CameraInputState ReadMouseState()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return default;
            }

            Vector2 pointerDelta = mouse.delta.ReadValue();
            Vector2 orbit = mouse.leftButton.isPressed ? pointerDelta : Vector2.zero;
            Vector2 pan = mouse.middleButton.isPressed || mouse.rightButton.isPressed
                ? pointerDelta
                : Vector2.zero;
            float zoom = mouse.scroll.ReadValue().y;
            return new CameraInputState(orbit, pan, zoom);
        }

        private CameraInputState ReadTouchState()
        {
            _activeTouches.Clear();
            Touchscreen touchscreen = Touchscreen.current;
            if (touchscreen == null)
            {
                _previousTouchCount = 0;
                return default;
            }

            foreach (TouchControl touch in touchscreen.touches)
            {
                if (touch.press.isPressed)
                {
                    _activeTouches.Add(touch);
                    if (_activeTouches.Count == 2)
                    {
                        break;
                    }
                }
            }

            int touchCount = _activeTouches.Count;
            if (touchCount != _previousTouchCount)
            {
                _previousTouchCount = touchCount;
                return default;
            }

            if (touchCount == 1)
            {
                return new CameraInputState(_activeTouches[0].delta.ReadValue(), Vector2.zero, 0f);
            }

            if (touchCount == 2)
            {
                TouchControl first = _activeTouches[0];
                TouchControl second = _activeTouches[1];
                Vector2 firstDelta = first.delta.ReadValue();
                Vector2 secondDelta = second.delta.ReadValue();
                Vector2 pan = (firstDelta + secondDelta) * 0.5f;

                Vector2 firstPosition = first.position.ReadValue();
                Vector2 secondPosition = second.position.ReadValue();
                float currentDistance = Vector2.Distance(firstPosition, secondPosition);
                float previousDistance = Vector2.Distance(
                    firstPosition - firstDelta,
                    secondPosition - secondDelta);

                return new CameraInputState(Vector2.zero, pan, currentDistance - previousDistance);
            }

            return default;
        }
    }
}
