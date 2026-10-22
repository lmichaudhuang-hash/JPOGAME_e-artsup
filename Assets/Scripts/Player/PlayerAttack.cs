using UnityEngine;

namespace JPO
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private GameObject swordPivot;
        [SerializeField] private float attackDuration = 0.3f;
        [SerializeField] private float cooldownDuration = 0.5f;
        [SerializeField] private float startAttackAngle = 90f;
        [SerializeField] private float endAttackAngle = -90f;

        private PlayerController _controller;
        private PlayerMovement _movement;
        private bool _isAttackButtonPressed;
        private Timer _cooldownTimer;
        private Timer _attackTimer;
        private FacingDirection _attackDirection;
        private bool _isUnderCooldown;
        private bool _isAttacking;

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _movement = GetComponent<PlayerMovement>();

            // Listen for attack inputs
            _controller.onAttack.AddListener(OnAttack);
            _controller.onStopAttack.AddListener(OnStopAttack);

            // Hide sword
            swordPivot.SetActive(false);
        }

        private void Update()
        {
            // Update the attack
            if (_isAttacking) AttackUpdate();

            // Handle end of attack timer
            if (_attackTimer.ExpiredAndWasRunning())
            {
                // Update attack state
                _attackTimer = Timer.None;
                _isAttacking = false;
            }

            // Handle end of cooldown timer
            if (_cooldownTimer.ExpiredAndWasRunning())
            {
                // Update cooldown state
                _cooldownTimer = Timer.None;
                _isUnderCooldown = false;
                // Hide sword visual
                swordPivot.SetActive(false);
                // Restart the attack if the attack button is still pressed
                if (_isAttackButtonPressed) AttackStart();
            }
        }

        private void OnDestroy()
        {
            if (!_controller) return;
            _controller.onAttack.RemoveListener(OnAttack);
            _controller.onStopAttack.RemoveListener(OnStopAttack);
        }

        /// <summary>
        /// Handles the attack input pressed
        /// </summary>
        private void OnAttack()
        {
            _isAttackButtonPressed = true;
            // Check if the player is already attacking
            // or if a cooldown is active before starting the attack
            if (_isAttacking || _isUnderCooldown) return;
            AttackStart();
        }

        /// <summary>
        /// Handles the attack input released
        /// </summary>
        private void OnStopAttack()
        {
            _isAttackButtonPressed = false;
        }

        /// <summary>
        /// Start the attack
        /// </summary>
        private void AttackStart()
        {
            // Update State
            _attackTimer = Timer.CreateFromSeconds(attackDuration);
            _cooldownTimer = Timer.CreateFromSeconds(cooldownDuration);
            _isAttacking = true;
            _isUnderCooldown = true;
            _attackDirection = _movement.FacingDirection;

            // Initialize sword visual
            swordPivot.transform.localEulerAngles = Vector3.forward * (
                _attackDirection == FacingDirection.Right
                    ? startAttackAngle
                    : -startAttackAngle
            );
            swordPivot.SetActive(true);
        }

        /// <summary>
        /// Update the attack
        /// </summary>
        private void AttackUpdate()
        {
            // Update sword angle and rotate arround the Z axis
            var progress = Mathf.Clamp01(_attackTimer.GetElapsedTime() / attackDuration);
            var start = _attackDirection == FacingDirection.Right
                ? startAttackAngle
                : -startAttackAngle;
            var end = _attackDirection == FacingDirection.Right
                ? endAttackAngle
                : -endAttackAngle;
            var currentAngle = Mathf.Lerp(start, end, progress);
            swordPivot.transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
        }
    }
}