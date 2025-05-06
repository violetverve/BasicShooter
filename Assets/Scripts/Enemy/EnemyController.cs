using UnityEngine;

namespace BasicShooter.Enemy
{
    /// <summary>
    /// Controls the overall behavior of the enemy, including movement and shooting.
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        private EnemyMotor _motor;
        private EnemyShooter _shooter;
        private Transform _target;

        private void Awake()
        {
            _motor = GetComponent<EnemyMotor>();
            _shooter = GetComponent<EnemyShooter>();
        }

        public void Initialize(Transform target)
        {
            _target = target;

            _motor.Initialize(target);
            _shooter.Initialize(target);
        }

        private void Update()
        {
            if (_target == null)
            {
                return;
            }

            _motor.FollowTarget();
            _motor.RotateTowardsTarget();

            if (_shooter.CanShoot())
            {
                _shooter.ShootAtTarget();
            }
        }
    }
}