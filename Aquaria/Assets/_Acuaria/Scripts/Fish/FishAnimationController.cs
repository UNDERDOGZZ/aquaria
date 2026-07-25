using UnityEngine;

namespace Acuaria.Fish
{
    [DisallowMultipleComponent]
    public sealed class FishAnimationController : MonoBehaviour
    {
        private static readonly int SwimSpeed = Animator.StringToHash("swimSpeed");
        private static readonly int Turn = Animator.StringToHash("turn");
        private static readonly int SwimState = Animator.StringToHash("Swim");

        [SerializeField] private FishMovement movement;
        [SerializeField] private Animator animator;

        public Animator Animator => animator;

        public void Configure(FishMovement fishMovement, Animator visualAnimator)
        {
            movement = fishMovement;
            animator = visualAnimator;
        }

        private void Awake()
        {
            if (movement == null) movement = GetComponent<FishMovement>();
            if (animator == null) animator = GetComponentInChildren<Animator>(true);
            if (animator == null) return;
            animator.applyRootMotion = false;
            animator.SetFloat(Turn, 0.5f);
            animator.CrossFade(SwimState, 0f, 0, Random.value);
        }

        private void Update()
        {
            if (animator == null || movement == null || movement.State == null) return;
            FishMovementSettings settings = movement.State.Species.MovementSettings;
            float range = Mathf.Max(0.01f, settings.MaximumSpeed - settings.MinimumSpeed);
            float normalizedSpeed = Mathf.Clamp01(
                (movement.State.Speed - settings.MinimumSpeed) / range);
            animator.SetFloat(SwimSpeed, normalizedSpeed);
        }
    }
}
