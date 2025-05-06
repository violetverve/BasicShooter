using UnityEngine;
using UnityEngine.AI;

namespace BasicShooter.Enemy
{
    /// <summary>
    /// Controls the movement and rotation behavior of the enemy using Unity's NavMeshAgent.
    /// </summary>
    public class EnemyMotor : MonoBehaviour
    {
        [SerializeField] private float _stopDistance = 5f;
        [SerializeField] private float _rotationSpeed = 5f;
        private NavMeshAgent _agent;
        private Transform _target;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void Initialize(Transform target)
        {
            _target = target;
        }

        public void FollowTarget()
        {
            if (_target == null) return;

            float distance = Vector3.Distance(transform.position, _target.position);

            if (distance > _stopDistance)
            {
                _agent.SetDestination(_target.position);
            }
            else
            {
                _agent.ResetPath();
            }
        }

        public void RotateTowardsTarget()
        {
            Vector3 direction = _target.position - transform.position;
            direction.y = 0; // Keep the enemy rotating on the y-axis (flat rotation)

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }
}