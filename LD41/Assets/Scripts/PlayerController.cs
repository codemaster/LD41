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
		if (Input.GetButtonUp("Fire1"))
		{
			RaycastHit hit;
			if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
			{
				_agent.SetDestination(hit.point);
			}
		}
	}
}
