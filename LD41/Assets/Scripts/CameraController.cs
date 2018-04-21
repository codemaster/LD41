using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for camera in gameplay
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
	/// <summary>
	/// Camera position offset
	/// </summary>
	[SerializeField]
	private Vector3 _offset;

	/// <summary>
	/// The amount of time to smoothly move the camera
	/// </summary>
	[SerializeField]
	private float smoothingTime = 1f;

	/// <summary>
	/// Maximum amount of zoom
	/// </summary>
	[SerializeField]
	private float zoomMax = 10f;

	/// <summary>
	/// Minimum amount of zoom
	/// </summary>
	[SerializeField]
	private float zoomMin = 30f;

	/// <summary>
	/// The size of a normal zoom
	/// </summary>
	[SerializeField]
	private float zoomSize = 30f;

	/// <summary>
	/// Targets to keep in view
	/// </summary>
	[SerializeField]
	private List<Transform> _targets = new List<Transform>();

	/// <summary>
	/// The camera component
	/// </summary>
	private Camera _camera;

	/// <summary>
	/// The velocity when moveing the camera
	/// </summary>
	private Vector3 _velocity;

	/// <summary>
	/// Adds a target to keep in view
	/// </summary>
	/// <param name="target">Target to keep in view</param>
	public void AddTarget(Transform target) => _targets.Add(target);

	/// <summary>
	/// Removes a target from view
	/// </summary>
	/// <param name="target">Target to no longer keep in view</param>
	public void RemoveTarget(Transform target) => _targets.Remove(target);

	/// <summary>
	/// Self-initialization
	/// </summary>
	private void Awake()
	{
		_camera = GetComponent<Camera>();
	}

	/// <summary>
	/// Updates after each frame
	/// </summary>
	private void LateUpdate()
	{
		if (_targets.Count == 0)
		{
			return;
		}

		// Calculate the center of all of the targets
		var bounds = new Bounds(_targets[0].position, Vector3.zero);
		foreach (var target in _targets)
		{
			bounds.Encapsulate(target.position);
		}

		// Smoothly move the camera
		transform.position = Vector3.SmoothDamp(transform.position, bounds.center + _offset,
			ref _velocity, smoothingTime);

		// Adjust FoV to simulate zooming
		_camera.fieldOfView = Mathf.Lerp(zoomMax, zoomMin, bounds.size.x / zoomSize);
	}
}
