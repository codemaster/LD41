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
		if (Input.GetButtonUp("Fire1"))
		{
			Shoot();
		}

		// Movement
		if (Input.GetButtonUp("Fire2"))
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

		// Note: Using Vector3.back, as our characters are technically backward.
		// TODO: Maybe adjust characters to go forward instead. NavMesh issue?
		var shootPos = transform.position + Vector3.back + Vector3.up;
		var enemyLayer = LayerMask.NameToLayer("Enemy");
		var layerMask = 1 << enemyLayer;

		// Check if our shotgun spread hit any enemies
		var colliders = Physics.OverlapSphere(shootPos, 2.5f, layerMask);
		if (colliders.Length == 0)
		{
			return;
		}

		// Enemies were hit!
		var enemies = colliders.Select(c => c.GetComponent<Enemy>()).ToList();
		// While we have enemies left...
		while (enemies.Count > 0)
		{
			try
			{
				// If any of the enemies are the next letter
				var foundEnemy = enemies.First(e => e.Letter == _wordController.GetNextLetter());
				// TODO: we obtained the letter!
				// TODO: kill the enemy
				// remove it from the listing
				enemies.Remove(foundEnemy);
			}
			catch
			{
				// Otherwise, penalty for the enemies left
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
