﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

/// <summary>
/// An enemy
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
	/// <summary>
	/// The letter for the enemy to show
	/// </summary>
	[SerializeField]
	public char _letter;

	/// <summary>
	/// The display for the letter
	/// </summary>
	[SerializeField]
	private TextMeshProUGUI _letterDisplay;

	/// <summary>
	/// How far away to pick a random position
	/// </summary>
	[SerializeField]
	private float _randomDistance = 5f;

	/// <summary>
	/// The NavMeshAgent
	/// </summary>
	private NavMeshAgent _agent;

	/// <summary>
	/// Coroutine holding area for picking a path
	/// </summary>
	private Coroutine _pathCoroutine;

	/// <summary>
	/// Getter and setter for the enemy's letter
	/// </summary>
	public char Letter {
		get { return _letter; }
		set
		{
			_letter = value;
			_letterDisplay.SetText(_letter.ToString());
		}
	}

	/// <summary>
	/// Self-initialization
	/// </summary>
	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
	}

	/// <summary>
	/// Per frame
	/// </summary>
	private void Update()
	{
		// Check if the current pathing is complete
		if (_agent.IsComplete() && _pathCoroutine == null)
		{
			_pathCoroutine = StartCoroutine(PickNewPath());
		}
	}

	/// <summary>
	/// Picks a new position to walk to
	/// </summary>
	/// <returns>Coroutine enumerator</returns>
	private IEnumerator PickNewPath()
	{
		// Pick a semi-random amount of time to wait
		yield return new WaitForSeconds(1f);

		// Pick a new path!
		var direction = (Random.insideUnitSphere * _randomDistance) + transform.position;
		NavMeshHit hit;
		
		NavMesh.SamplePosition(direction, out hit, _randomDistance, -1);
		_agent.SetDestination(hit.position);

		// Clear out the coroutine at the end
		_pathCoroutine = null;
	}

	/// <summary>
	/// Pool for creating an enemy
	/// </summary>
	public class Pool : MonoMemoryPool<char, Enemy>
	{
		/// <summary>
		/// The nav mesh surface
		/// </summary>
		[Inject]
		private NavMeshSurface _navMeshSurface;

		/// <summary>
		/// Camera controller
		/// </summary>
		[Inject]
		private CameraController _cameraController;

		/// <summary>
		/// Reinitializing a previously used enemy
		/// </summary>
		/// <param name="enemy">The enemy object we are reinitializing</param>
		protected override void Reinitialize(char letter, Enemy enemy)
		{
			// Set the letter
			enemy.Letter = letter;

			// Pick a random position to spawn the enemy
			var scaledSize = Vector3.Scale(_navMeshSurface.size,
				_navMeshSurface.gameObject.transform.localScale);
			var maxDistance = Mathf.Min(scaledSize.x, scaledSize.y);
			var direction = (Random.insideUnitSphere * maxDistance) +
				_navMeshSurface.center;

			NavMeshHit hit;
			NavMesh.SamplePosition(direction, out hit, maxDistance, -1);
			enemy.transform.position = hit.position;
		}

		/// <summary>
		/// When an enemy is spawned
		/// </summary>
		/// <param name="enemy">The enemy</param>
		protected override void OnSpawned(Enemy enemy)
		{
			_cameraController.AddTarget(enemy.transform);
			base.OnSpawned(enemy);
		}

		/// <summary>
		/// When an enemy is despawned
		/// </summary>
		/// <param name="enemy">The enemy</param>
		protected override void OnDespawned(Enemy enemy)
		{
			_cameraController.RemoveTarget(enemy.transform);
			base.OnDespawned(enemy);
		}
	}
}
