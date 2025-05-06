using UnityEngine;
using UnityEngine.InputSystem;

namespace BasicShooter.Player
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    
    /// <summary>
    /// Controls the player's movement, gravity, and rotation relative to the camera.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _playerSpeed = 2.0f;
        [SerializeField] private float _gravityValue = -9.81f;
        [SerializeField] private float _rotationSpeed = 2f;
        private CharacterController _controller;
        private PlayerInput _playerInput;
        private Vector3 _playerVelocity;
        private bool _groundedPlayer;
        private Transform _cameraTransform;
        private InputAction _moveAction;
        private const string MoveAction = "Move";

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();

            _cameraTransform = Camera.main.transform;
            _moveAction = _playerInput.actions[MoveAction];
        }

        private void Update()
        {
            HandleGroundCheck();
            HandleMovement();
            ApplyGravity();
            ApplyFinalMovement();
            RotateTowardsCamera();
        }

        private void HandleGroundCheck()
        {
            _groundedPlayer = _controller.isGrounded;
            if (_groundedPlayer && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0f;
            }
        }

        private void HandleMovement()
        {
            Vector2 input = _moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, input.y);
            move = Vector3.ClampMagnitude(move, 1f);
            move = move.x * _cameraTransform.right.normalized + move.z * _cameraTransform.forward.normalized;
            move.y = 0f;

            _controller.Move(move * _playerSpeed * Time.deltaTime);
        }

        private void ApplyGravity()
        {
            _playerVelocity.y += _gravityValue * Time.deltaTime;
        }

        private void ApplyFinalMovement()
        {
            _controller.Move(_playerVelocity * Time.deltaTime);
        }

        private void RotateTowardsCamera()
        {
            float targetAngle = _cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }
}
