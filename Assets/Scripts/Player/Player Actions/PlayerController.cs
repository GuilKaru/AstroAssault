using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AstroAssault
{

	[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields

		[Header("Player Movement")]
		[SerializeField]
		private float _moveSpeed = 5f;

		[Header("Player Shoot")]
		[SerializeField]
		private GameObject _bulletPrefab;
		[SerializeField]
		private Transform _gunBarrelTransform;
		[SerializeField]
		private Transform _bulletParent;
		[SerializeField]
		private float _automaticFireRate = 0.1f;
		[SerializeField]
		private float _bulletHitMissDistance = 25f;

		[Header("Shield Buff")]
		[SerializeField]
		private GameObject _playerShield;

		[Header("Shotgun Settings")]
		[SerializeField] private int _shotgunPelletCount = 3;
		[SerializeField] private float[] _shotgunSpreadAngles = { -30f, 0f, 30f };

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		#endregion

		//Private Variables
		#region Private Variables
		private Vector2 _moveInput;
		private Rigidbody2D _rb;
		private PlayerInput _playerInput;
		private InputAction _moveAction;
		private InputAction _shootAction;
		private int _currentShootMode = 1; // 1: Automatic, 2: Shotgun
		private Coroutine _shootingCoroutine;
		private bool _wasMoving = false;
		private bool _isShooting;
		#endregion

		//References
		#region References

		//Animations
		private string _currentState;
		private string _idleAnim = "Player_Idle";
		private string _walkingAnim = "Player_Walk";
		private string _shootStartAnim = "Player_Shoot_Start";
		private string _shootingAnim = "Player_Shoot_Loop";
		private string _shootEndAnim = "Player_Shoot_End";

		#endregion

		// Initialization
		#region Initialization

		private void Awake()
		{
			// Initialize components
			_rb = GetComponent<Rigidbody2D>();
			if (_rb == null)
			{
				Debug.LogError("Rigidbody2D component is missing!");
			}

			_playerInput = GetComponent<PlayerInput>();
			if (_playerInput == null)
			{
				Debug.LogError("PlayerInput component is missing!");
			}

			// Setup input actions
			_moveAction = _playerInput.actions["Move"];
			_shootAction = _playerInput.actions["Shoot"];

			ChangeAnimationState(_idleAnim);
		}

		private void OnEnable()
		{
			_moveAction.started += Move;
			_moveAction.performed += Move;
			_moveAction.canceled += Move;

			// Hook up shoot action
			_shootAction.started += OnShootStart;
			_shootAction.canceled += OnShootStop;

			// Listen for key inputs to change shooting mode
			_playerInput.actions["ChangeToAutomatic"].performed += _ => ChangeShootingMode(1);
			_playerInput.actions["ChangeToShotgun"].performed += _ => ChangeShootingMode(2);
		}

		private void OnDisable()
		{
			_moveAction.started -= Move;
			_moveAction.performed -= Move;
			_moveAction.canceled -= Move;

			// Unhook shoot action
			_shootAction.started -= OnShootStart;
			_shootAction.canceled -= OnShootStop;

			_playerInput.actions["ChangeToAutomatic"].performed -= _ => ChangeShootingMode(1);
			_playerInput.actions["ChangeToShotgun"].performed -= _ => ChangeShootingMode(2);
		}

		#endregion

		//Movement
		#region Movement
		private void FixedUpdate()
		{
            if (!GameManager.gameManager.gameStarted) return;

            MovePlayer();
			UpdateAnimationState();
		}

		private void Move(InputAction.CallbackContext context)
		{
			_moveInput = context.ReadValue<Vector2>();
		}

		private void MovePlayer()
		{
			Vector2 movement = _moveInput * _moveSpeed * Time.fixedDeltaTime;
			_rb.MovePosition(_rb.position + movement);
		}

		#endregion

		//Shooting
		#region Shooting
		private void OnShootStart(InputAction.CallbackContext context)
		{
            if (!GameManager.gameManager.gameStarted) return;

            if (_shootingCoroutine == null)
			{
				ChangeAnimationState(_shootStartAnim);
				_isShooting = true;
				_shootingCoroutine = StartCoroutine(ShootingCoroutine());

			}
		}

		private void OnShootStop(InputAction.CallbackContext context)
		{
			if (_shootingCoroutine != null)
			{
				StopCoroutine(_shootingCoroutine);
				_shootingCoroutine = null;
			}

			ChangeAnimationState(_shootEndAnim);
		}

		private IEnumerator ShootingCoroutine()
		{
			while (_isShooting)
			{
				if (_currentShootMode == 1)
				{
					AutomaticShoot();
					
				}
				else if (_currentShootMode == 2)
				{
					ShotgunShoot();
				}

				yield return new WaitForSeconds(_automaticFireRate);
			}
		}

		private void AutomaticShoot()
		{
			GameObject bullet = Instantiate(_bulletPrefab, _gunBarrelTransform.position, Quaternion.identity, _bulletParent);
			BulletController bulletController = bullet.GetComponent<BulletController>();
			if (bulletController != null)
			{
				bulletController.target = _gunBarrelTransform.position + _gunBarrelTransform.up * _bulletHitMissDistance;
			}
		}

		private void ShotgunShoot()
		{
			for (int i = 0; i < _shotgunPelletCount; i++)
			{
				float spread = _shotgunSpreadAngles[i];
				Quaternion spreadRotation = Quaternion.Euler(0, 0, spread);

				GameObject bullet = Instantiate(_bulletPrefab, _gunBarrelTransform.position, spreadRotation * Quaternion.identity, _bulletParent);
				BulletController bulletController = bullet.GetComponent<BulletController>();
				if (bulletController != null)
				{
					bulletController.target = _gunBarrelTransform.position + (spreadRotation * _gunBarrelTransform.up) * _bulletHitMissDistance;
				}
			}
		}

		public void ChangeShootingMode(int mode)
		{
			_currentShootMode = mode;
			Debug.Log($"Shooting mode changed to: {(_currentShootMode == 1 ? "Automatic" : "Shotgun")}");
		}
		#endregion

		//Shield
		#region Shield
		public void ActivateShield(bool isActive)
		{
			// Toggle the shield's active state (e.g., enable or disable the shield visual/collider)
			_playerShield.SetActive(isActive);  // Assuming you have a shield object or visual
		}
		#endregion

		//Collision
		#region Collision

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if(collision.gameObject.CompareTag("Border"))
			{
				_rb.MovePosition(Vector2.zero);
			}

		}
		#endregion

		//Player Animations
		#region Player Animations
		private void ChangeAnimationState(string newState)
		{
			// Avoid transitioning to the same animation
			if (_currentState == newState) return;

			// Play the new animation
			_animator.Play(newState);

			// Update the current state
			_currentState = newState;

		}

		private void UpdateAnimationState()
		{
			if (_isShooting) return; // Shooting animations take priority

			bool isCurrentlyMoving = _moveInput != Vector2.zero;

			if (isCurrentlyMoving)
			{
				ChangeAnimationState(_walkingAnim); // Walk if moving
				//_wasMoving = true;
			}
			else
			{
				ChangeAnimationState(_idleAnim); // Idle if stationary
				//_wasMoving = false;
			}
		}

		#endregion


	}
}

