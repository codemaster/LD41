using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for managing sound
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
	/// <summary>
	/// SFX for Gunshot
	/// </summary>
	public AudioClip Gunshot;

	/// <summary>
	/// SFX for Gaining points
	/// </summary>
	public AudioClip GainPoints;

	/// <summary>
	/// SFX for losing points
	/// </summary>
	public AudioClip LosePoints;

	/// <summary>
	/// SFX for clicking 'Play Again' button
	/// </summary>
	public AudioClip PlayAgain;

	/// <summary>
	/// SFX for an enemy getting hurt
	/// </summary>
	public AudioClip EnemyHurt;

	/// <summary>
	/// SFX for enemy groaning (#1)
	/// </summary>
	public AudioClip EnemyGroan1;

	/// <summary>
	/// SFX for enemy groaning (#2)
	/// </summary>
	public AudioClip EnemyGroan2;

	/// <summary>
	/// The BGM audio source
	/// </summary>
	private AudioSource _bgmSource;

	/// <summary>
	/// Audio source for SFX
	/// </summary>
	[SerializeField]
	private AudioSource _sfxSource;

	/// <summary>
	/// Minimum BGM pitch
	/// </summary>
	[SerializeField]
	private float _minBgmPitch = 0.5f;

	/// <summary>
	/// Maximum BGM pitch
	/// </summary>
	[SerializeField]
	private float _maxBgmPitch = 1f;

	/// <summary>
	/// Queue for SFX
	/// </summary>
	private readonly Queue<AudioClip> _sfxQueue = new Queue<AudioClip>();

	/// <summary>
	/// Self-initialization
	/// </summary>
	private void Awake()
	{
		_bgmSource = GetComponent<AudioSource>();
		_bgmSource.loop = false;
		// Generate a random pitch to set it to
		_bgmSource.pitch = Random.Range(_minBgmPitch, _maxBgmPitch);
	}

	/// <summary>
	/// Update per frame
	/// </summary>
	private void Update()
	{
		// Detect if the BGM finished
		if (!_bgmSource.isPlaying)
		{
			// Generate a random pitch to set it to
			_bgmSource.pitch = Random.Range(_minBgmPitch, _maxBgmPitch);
			// Play again
			_bgmSource.Play();
		}

		// Detect if SFX is playing or needed
		if (!_sfxSource.isPlaying && _sfxQueue.Count > 0)
		{
			_sfxSource.PlayOneShot(_sfxQueue.Dequeue());
		}
	}

	/// <summary>
	/// Plays a SFX once
	/// </summary>
	/// <param name="clip"></param>
	public void PlaySFX(AudioClip clip)
	{
		_sfxQueue.Enqueue(clip);
	}
}
