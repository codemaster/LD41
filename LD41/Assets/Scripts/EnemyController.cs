using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Controller for spawning and destroying enemies
/// </summary>
public class EnemyController : MonoBehaviour
{
	/// <summary>
	/// The word controller
	/// </summary>
	[Inject]
	private WordController _wordController;

	/// <summary>
	/// The enemy pool
	/// </summary>
	[Inject]
	private Enemy.Pool _enemyPool;

	/// <summary>
	/// Creates a new enemy with a provided letter
	/// </summary>
	/// <param name="letter">The letter the enemy has</param>
	public void CreateEnemy(char letter)
	{
		// Spawn a new enemy with the provided letter
		_enemyPool.Spawn(letter);
	}

	/// <summary>
	/// Destroys an enemy
	/// </summary>
	/// <param name="enemy">The enemy to destroy</param>
	public void DestroyEnemy(Enemy enemy)
	{
		// Despawn the enemy
		_enemyPool.Despawn(enemy);
	}

	/// <summary>
	/// Creates multiple enemies from letters
	/// </summary>
	/// <param name="letters">The letters to create the enemies from</param>
	public void CreateEnemies(IEnumerable<char> letters)
	{
		foreach(var letter in letters)
		{
			CreateEnemy(letter);
		}
	}

	/// <summary>
	/// Destroys multiple enemies
	/// </summary>
	/// <param name="enemies">The enemies</param>
	public void DestroyEnemies(IEnumerable<Enemy> enemies)
	{
		foreach (var enemy in enemies)
		{
			DestroyEnemy(enemy);
		}
	}
}
