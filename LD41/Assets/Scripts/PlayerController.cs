using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

/// <summary>
/// Controller for the player
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
	/// <summary>
	/// The nav mesh agent component
	/// </summary>
	private NavMeshAgent _agent;

	/// <summary>
	/// The main camera
	/// </summary>
	private Camera _mainCamera;

	/// <summary>
	/// The gun particle to show when shooting
	/// </summary>
	[SerializeField]
	private ParticleSystem _gunParticle;

	/// <summary>
	/// The shotgun scatter range
	/// </summary>
	[SerializeField]
	private float _shotgunScatterRange = 3f;

	/// <summary>
	/// The enemy controller
	/// </summary>
	[Inject]
	private EnemyController _enemyController;

	/// <summary>
	/// The game controller
	/// </summary>
	[Inject]
	private GameController _gameController;

	/// <summary>
	/// The score controller
	/// </summary>
	[Inject]
	private ScoreController _scoreController;

	/// <summary>
	/// The sound controller
	/// </summary>
	[Inject]
	private SoundController _soundController;

	/// <summary>
	/// The word controller
	/// </summary>
	[Inject]
	private WordController _wordController;

	/// <summary>
	/// Initializes the controller with injections
	/// </summary>
	[Inject]
	public void Initialize(CameraController cameraController)
	{
		cameraController.AddTarget(transform);
	}

	/// <summary>
	/// Self-initialization
	/// </summary>
	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_mainCamera = Camera.main;
	}

	/// <summary>
	/// Each frame update
	/// </summary>
	private void Update()
	{
		// Shooting
		if (Input.GetButtonUp("Fire2") || Input.GetButtonUp("Jump"))
		{
			Shoot();
		}

		// Movement
		if (Input.GetButtonUp("Fire1"))
		{
			Move();
		}
	}

	/// <summary>
	/// Shoots at an enemy
	/// </summary>
	private void Shoot()
	{
		// If the particle system is already playing, don't shoot again
		if (_gunParticle.isPlaying)
		{
			return;
		}

		// Show the particle
		_gunParticle.Play();

		// Play the sound
		_soundController.PlaySFX(_soundController.Gunshot);

		// Note: Using Vector3.back, as our characters are technically backward.
		// TODO: Maybe adjust characters to go forward instead. NavMesh issue?
		var shootPos = transform.position + Vector3.back + Vector3.up;
		var enemyLayer = LayerMask.NameToLayer("Enemy");
		var layerMask = 1 << enemyLayer;

		// Check if our shotgun spread hit any enemies
		var colliders = Physics.OverlapSphere(shootPos, _shotgunScatterRange, layerMask);
		if (colliders.Length == 0)
		{
			return;
		}

		// Enemies were hit!
		var enemies = colliders.Select(c => c.GetComponent<Enemy>()).ToList();

		// If we hit any enemies
		if (enemies.Count > 0)
		{
			// Play the enemy hurt sound
			_soundController.PlaySFX(_soundController.EnemyHurt);
		}

		// While we have enemies left...
		while (enemies.Count > 0)
		{
			try
			{
				// If any of the enemies are the next letter
				var foundEnemy = enemies.First(e => e.Letter == _wordController.GetNextLetter());
				// We obtained the letter!
				_wordController.ObtainLetter();
				// Play the sound
				_soundController.PlaySFX(_soundController.GainPoints);
				// Increment the score!
				_scoreController.Score += _scoreController.ScorePerLetterIncrement *
					_wordController.GetNumberObtainedLetters();
				// Remove the enemy from the listing
				enemies.Remove(foundEnemy);
				// Destroy the enemy
				_enemyController.DestroyEnemy(foundEnemy);
				// Go to the next letter!
				_gameController.NextLetter();
			}
			catch
			{
				// Otherwise, penalty for the enemies left
				var numEnemies = enemies.Count;
				// Play the sound
				_soundController.PlaySFX(_soundController.LosePoints);
				// Decrement the score!
				_scoreController.Score -= _scoreController.ScoreDecrementPerBadEnemy *
					_wordController.GetNumberObtainedLetters();
				// Destroy the enemies
				_enemyController.DestroyEnemies(enemies);
				enemies.Clear();
			}
		}
	}

	/// <summary>
	/// Moves the character to where the mouse is
	/// </summary>
	private void Move()
	{
		RaycastHit hit;
		if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
		{
			_agent.SetDestination(hit.point);
		}
	}
}
