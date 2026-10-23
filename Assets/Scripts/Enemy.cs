using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace JPO
{
    public class Enemy : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Vector2 firePositionOffset;
        [SerializeField] private int projectilePerWave = 1;
        [SerializeField] private float waveCooldownDuration = 2f;
        [SerializeField] private float delayBetweenProjectile = 1f;

        [Header("Player Detection")]
        [SerializeField] private float playerDetectionRadius = 5f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Damage Feedback")]
        [SerializeField] private SpriteRenderer enemySprite;
        [SerializeField] private float damageBlinkFadeDuration = 0.1f;
        [SerializeField] private float damageBlinkInterval = 0.2f;
        [SerializeField] private AudioClip hitAudio;
        [SerializeField] private AudioClip deathAudio;

        [Header("Item Drop")]
        [SerializeField] private Candy candyPrefab;
        [SerializeField, Range(0f, 1f)] private float itemDropChance = 0.5f;

        private bool _hasDetectedPlayer;
        private bool _waveIsActive; // If the enemy is currently firing
        private int _waveFiredProjectiles; // Number of projectiles fired during this wave
        private Timer _fireTimer; // Timer between each projectile
        private Timer _waveCooldownTimer; // Timer used to wait for the next fire wave
        private HealthSystem _healthSystem;
        private Vector2 _lastPlayerPosition;
        private Sequence _blinkSequence;
        private Tween _fireFeedbackTween;
        private AudioSource _source;

        private void Awake()
        {
            _healthSystem = GetComponent<HealthSystem>();
            _source = GetComponent<AudioSource>();
            _healthSystem.onDamageReceived.AddListener(OnDamageTaken);
            _healthSystem.onInvulnerabilityEnd.AddListener(OnInvulnerabilityEnd);
        }

        private void Update()
        {
            if (!_healthSystem.IsAlive) return;
            // Look in the player direction
            if (_hasDetectedPlayer)
            {
                enemySprite.flipX = _lastPlayerPosition.x < transform.position.x;
            }

            // Start the fire wave
            if (_hasDetectedPlayer && !_waveIsActive && !_waveCooldownTimer.IsRunning)
            {
                _waveIsActive = true;
                _waveFiredProjectiles = 0;
                var dirToPlayer = (_lastPlayerPosition - (Vector2)transform.position).normalized;
                FireProjectile(dirToPlayer);
            }

            // Fire a new projectile at the end of the timer
            if (_fireTimer.ExpiredAndWasRunning())
            {
                var dirToPlayer = (_lastPlayerPosition - (Vector2)transform.position).normalized;
                FireProjectile(dirToPlayer);
            }

            // Handle end of wave cooldown timer
            if (_waveCooldownTimer.ExpiredAndWasRunning())
            {
                _waveCooldownTimer = Timer.None;
            }
        }

        private void FixedUpdate()
        {
            if (!_healthSystem.IsAlive) return;
            // Detect the player
            var playerCol = Physics2D.OverlapCircle(transform.position, playerDetectionRadius, playerLayer);
            _hasDetectedPlayer = playerCol != null;
            if (_hasDetectedPlayer) _lastPlayerPosition = playerCol.transform.position;
        }

        private void OnDestroy()
        {
            _fireFeedbackTween?.Kill();
            _blinkSequence?.Kill();
        }

        /// <summary>
        /// Instantiate and setup a projectile
        /// </summary>
        private void FireProjectile(Vector2 direction)
        {
            var pos = transform.TransformPoint(firePositionOffset);
            var proj = ObjectPooler.Instance.InstantiatePrefab(projectilePrefab, pos, Quaternion.identity);
            proj.Initialize(gameObject, direction);
            _waveFiredProjectiles++;

            // Play animation
            _fireFeedbackTween?.Kill();
            _fireFeedbackTween = enemySprite.transform.DOPunchScale(new Vector3(1f, -1f) * 0.5f, 0.3f)
                .OnUpdate(() =>
                {
                    // Limit the sprite scale
                    var scale = enemySprite.transform.localScale;
                    scale.x = Mathf.Clamp(scale.x, 1f, 1.5f);
                    scale.y = Mathf.Clamp(scale.y, 0.5f, 1f);
                    enemySprite.transform.localScale = scale;
                })
                // Reset the sprite scale
                .OnComplete(() => enemySprite.transform.localScale = Vector3.one);

            // Start the timer for the next projectile or stop the wave
            if (_waveFiredProjectiles < projectilePerWave)
            {
                _fireTimer = Timer.CreateFromSeconds(delayBetweenProjectile);
            }
            else
            {
                _waveIsActive = false;
                _fireTimer = Timer.None;
                _waveCooldownTimer = Timer.CreateFromSeconds(waveCooldownDuration);
            }
        }

        /// <summary>
        /// When the player takes a damage, play hit feedback
        /// </summary>
        private void OnDamageTaken(int health, DamageInfo _)
        {
            // Stop the fire timer on death
            if (health <= 0)
            {
                _fireTimer = Timer.None;
                // Drop health item
                if (itemDropChance > 0 && itemDropChance >= Random.value)
                {
                    Instantiate(candyPrefab, transform.position, Quaternion.identity);
                }
                // Play death audio
                _source.PlayOneShot(deathAudio, 0.25f);
            }

            // Setup and play the blink feedback with a DOTween sequence
            _blinkSequence?.Kill();
            _blinkSequence = DOTween.Sequence().SetLoops(-1);
            _blinkSequence.Append(enemySprite.DOFade(0f, damageBlinkFadeDuration));
            _blinkSequence.AppendInterval(damageBlinkInterval);
            _blinkSequence.Append(enemySprite.DOFade(1f, damageBlinkFadeDuration));
            _blinkSequence.AppendInterval(damageBlinkInterval);
            _blinkSequence.Play();
            enemySprite.transform.DOPunchScale(new Vector3(1f, -1f), 0.3f)
                .OnComplete(() => enemySprite.transform.localScale = Vector3.one);

            // Play audio
            _source.PlayOneShot(hitAudio, 0.25f);
        }

        /// <summary>
        /// Stop the blink feedback at the end of invulnerability
        /// </summary>
        private void OnInvulnerabilityEnd()
        {
            _blinkSequence?.Kill();
            // Reset sprite alpha to 1
            enemySprite.SetAlpha(1f);
            // Cleanup sequence
            _blinkSequence = null;
        }

        private void OnDrawGizmosSelected()
        {
            // Set the circles color
            Gizmos.color = Color.red;
            // Draw the projectile spawn point
            Gizmos.DrawWireSphere(transform.TransformPoint(firePositionOffset), 0.1f);
            // Draw the player detection area
            Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
        }
    }
}