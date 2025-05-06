using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using BasicShooter.Enemy;

namespace BasicShooter.Particles
{
    /// <summary>
    /// Manages pooled hit particle effects caused by enemy shots.
    /// </summary>
    public class HitParticleManager : MonoBehaviour
    {
        [SerializeField] private GameObject _hitEffectPrefab;
        [SerializeField] private int _defaultPoolCapacity = 10;
        [SerializeField] private int _maxPoolCapacity = 50;
        private ObjectPool<GameObject> _hitEffectPool;
        private float _duration;

        private void Awake()
        {
            _hitEffectPool = new ObjectPool<GameObject>(
                CreateHitEffect,
                OnGetHitEffect,
                OnReleaseHitEffect,
                OnDestroyHitEffect,
                collectionCheck: true,
                defaultCapacity: _defaultPoolCapacity,
                maxSize: _maxPoolCapacity
            );

            _duration = _hitEffectPrefab.GetComponent<ParticleSystem>().main.duration;
        }

        private void OnEnable()
        {
            EnemyShooter.Shot += HandleShot;
        }

        private void OnDisable()
        {
            EnemyShooter.Shot -= HandleShot;
        }

        private void HandleShot(Vector3 source, Vector3 target)
        {
            SpawnHitEffect(source);
            SpawnHitEffect(target);
        }

        private GameObject CreateHitEffect()
        {
            GameObject hitEffect = Instantiate(_hitEffectPrefab);
            hitEffect.SetActive(false);
            return hitEffect;
        }

        private void OnGetHitEffect(GameObject hitEffect)
        {
            hitEffect.SetActive(true);
        }

        private void OnReleaseHitEffect(GameObject hitEffect)
        {
            hitEffect.SetActive(false);
        }

        private void OnDestroyHitEffect(GameObject hitEffect)
        {
            Destroy(hitEffect);
        }

        public void SpawnHitEffect(Vector3 position)
        {
            GameObject hitEffect = _hitEffectPool.Get();
            hitEffect.transform.position = position;
            StartCoroutine(DisableHitEffectAfterDuration(hitEffect));
        }

        private IEnumerator DisableHitEffectAfterDuration(GameObject hitEffect)
        {
            yield return new WaitForSeconds(_duration);
            _hitEffectPool.Release(hitEffect);
        }
    }
}