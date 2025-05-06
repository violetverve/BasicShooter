using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace BasicShooter.Enemy
{
    /// <summary>
    /// Manages the spawning, tracking, and removal of enemies.
    /// </summary
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private Transform _target;
        [SerializeField] private float _minSpawnDistance = 10f;
        [SerializeField] private float _maxSpawnDistance = 20f;
        [SerializeField] private int _findSpawnPositionAttempts = 20;
        [SerializeField] private float _minEnemySpacing = 2f;
        [SerializeField] private int _defaultPoolCapacity = 10;
        [SerializeField] private int _maxPoolSize = 50;

        private List<GameObject> _enemies = new List<GameObject>();
        private ObjectPool<GameObject> _enemyPool;
        private LayerMask _enemyLayerMask;
        private const string EnemiesMask = "Enemies";

        private void Awake()
        {
            _enemyPool = new ObjectPool<GameObject>(
                CreateEnemy,
                null,
                OnReleaseEnemy,
                OnDestroyEnemy,
                collectionCheck: true,
                defaultCapacity: _defaultPoolCapacity,
                maxSize: _maxPoolSize
            );

            _enemyLayerMask = LayerMask.GetMask(EnemiesMask);
        }

        private GameObject CreateEnemy()
        {
            GameObject enemy = Instantiate(_enemyPrefab);
            enemy.SetActive(false);
            return enemy;
        }

        private void OnReleaseEnemy(GameObject enemy)
        {
            enemy.SetActive(false);
        }

        private void OnDestroyEnemy(GameObject enemy)
        {
            Destroy(enemy);
        }

        private bool TryFindSpawnPosition(out Vector3 spawnPosition)
        {
            for (int attempt = 0; attempt < _findSpawnPositionAttempts; attempt++)
            {
                Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(_minSpawnDistance, _maxSpawnDistance);
                Vector3 candidatePosition = _target.position + new Vector3(randomCircle.x, 0, randomCircle.y);

                if (!Physics.CheckSphere(candidatePosition, _minEnemySpacing, _enemyLayerMask))
                {
                    spawnPosition = candidatePosition;
                    return true;
                }
            }

            spawnPosition = Vector3.zero;
            return false;
        }

        public void SpawnEnemy()
        {
            if (!TryFindSpawnPosition(out Vector3 spawnPosition))
            {
                return;
            }

            GameObject enemy = _enemyPool.Get();
            enemy.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
            enemy.SetActive(true);

            enemy.GetComponent<EnemyController>().Initialize(_target);
            _enemies.Add(enemy);
        }

        public void RemoveNearestEnemy()
        {
            GameObject nearest = null;
            float minDistance = float.MaxValue;

            foreach (var enemy in _enemies)
            {
                if (enemy == null) continue;

                float dist = Vector3.Distance(_target.position, enemy.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = enemy;
                }
            }

            if (nearest != null)
            {
                _enemies.Remove(nearest);
                _enemyPool.Release(nearest);
            }
        }

        public void OnSpawnEnemy(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                SpawnEnemy();
            }
        }

        public void OnRemoveNearestEnemy(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                RemoveNearestEnemy();
            }
        }
    }
}