using System;
using UnityEngine;
using BasicShooter.Player;

namespace BasicShooter.Enemy
{
    /// <summary>
    /// Controls the shooting behavior of an enemy.
    /// </summary>
    public class EnemyShooter : MonoBehaviour
    {
        [SerializeField] private Transform _shootingPoint;
        [SerializeField] private float _shootingRange = 20f;
        [SerializeField] private float _shootInterval = 2f;
        private Transform _target;
        private float _lastShotTime;

        public static event Action<Vector3, Vector3> Shot;

        public void Initialize(Transform target)
        {
            _target = target;
            ResetShootCooldown();
        }

        private bool TargetInRange()
        {
            return Vector3.Distance(transform.position, _target.position) <= _shootingRange;
        }

        private bool HasLineOfSight()
        {
            Vector3 directionToTarget = (_target.position - _shootingPoint.position).normalized;

            if (Physics.Raycast(_shootingPoint.position, directionToTarget, out RaycastHit hit, _shootingRange))
            {
                return hit.transform == _target;
            }

            return false;
        }

        private bool ShootIntervalPassed()
        {
            return Time.time - _lastShotTime >= _shootInterval;
        }

        public bool CanShoot()
        {
            return TargetInRange() && HasLineOfSight() && ShootIntervalPassed();
        }

        public void ShootAtTarget()
        {
            _lastShotTime = Time.time;

            Vector3 directionToPlayer = (_target.position - _shootingPoint.position).normalized;

            if (Physics.Raycast(_shootingPoint.position, directionToPlayer, out RaycastHit hit))
            {
                Shot?.Invoke(_shootingPoint.position, hit.point);
                 
                if (hit.collider.TryGetComponent<Health>(out var targetHealth))
                {
                    targetHealth.TakeDamage();
                }
            }
        }

        private void ResetShootCooldown()
        {
            _lastShotTime = Time.time;
        }
    }
}