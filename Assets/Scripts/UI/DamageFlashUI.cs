using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using BasicShooter.Player;

namespace BasicShooter.UI
{
    /// <summary>
    /// Manages an overlay flash on the UI when the player's health changes for visual feedback on damage.
    /// </summary>
    public class DamageFlashUI : MonoBehaviour
    {
        [SerializeField] private Health _playerHealth;
        [SerializeField] private Image _redOverlayImage;
        [SerializeField] private float _flashDuration = 0.2f;
        [SerializeField] private float _maxAlpha = 0.5f;
        private Coroutine _flashCoroutine;

        private void Start()
        {
            _playerHealth.OnHealthChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            _playerHealth.OnHealthChanged -= OnHealthChanged;
        }

        private void SetAlpha(float alpha)
        {
            if (_redOverlayImage != null)
            {
                Color c = _redOverlayImage.color;
                c.a = alpha;
                _redOverlayImage.color = c;
            }
        }

        private void OnHealthChanged()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }

            _flashCoroutine = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            float elapsed = 0f;
            SetAlpha(_maxAlpha);

            while (elapsed < _flashDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(_maxAlpha, 0f, elapsed / _flashDuration);
                SetAlpha(alpha);
                yield return null;
            }

            SetAlpha(0f);
            _flashCoroutine = null;
        }

    }
}