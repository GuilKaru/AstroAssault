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

		[Header("Shotgun Settings")]
		[SerializeField] private int _shotgunPelletCount = 3;
		[SerializeField] private float[] _shotgunSpreadAngles = { -30f, 0f, 30f };
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
			MovePlayer();
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
			if (_shootingCoroutine == null)
			{
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
		}

		private IEnumerator ShootingCoroutine()
		{
			while (true)
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

		private void ChangeShootingMode(int mode)
		{
			_currentShootMode = mode;
			Debug.Log($"Shooting mode changed to: {(_currentShootMode == 1 ? "Automatic" : "Shotgun")}");
		}
		#endregion
	}
}

