using UnityEngine;
using Zenject;

/// <summary>
/// Controller for the gameplay itself
/// </summary>
public class GameController : MonoBehaviour
{
	/// <summary>
	/// The exponential base
	/// </summary>
	[SerializeField]
	private float _expoBase = 1.25f;

	/// <summary>
	/// The word controller
	/// </summary>
	[Inject]
	private WordController _wordController;

	/// <summary>
	/// The enemy controller
	/// </summary>
	[Inject]
	private EnemyController _enemyController;

	/// <summary>
	/// Go to the next letter
	/// </summary>
	public void NextLetter()
	{
		// If we have more letters, spawn more enemies!
		var numLettersObtained = _wordController.GetNumberObtainedLetters();
		var numEnemies = Mathf.Pow(_expoBase, numLettersObtained);

		// Spawn real letter enemy
		var nextLetter = _wordController.GetNextLetter();
		_enemyController.CreateEnemy(nextLetter);
		var unobtainedLetters = _wordController.GetUnobtainedLetters();

		// Spawn dud letter enemies
		for (int i = 1; i < numEnemies; ++i)
		{
			char randomLetter;
			do
			{
				randomLetter = (char)Random.Range((float)'A', 'Z');
			} while (randomLetter == nextLetter || unobtainedLetters.Contains(randomLetter));

			_enemyController.CreateEnemy(randomLetter);
		}
	}

	/// <summary>
	/// Initialize with reference to others
	/// </summary>
	private void Start()
	{
		// Pick a word
		_wordController.SetWord("lolwut");
		// Spawn the first enemy
		Debug.Log($"Creating an enemy for letter {_wordController.GetNextLetter()}");
		_enemyController.CreateEnemy(_wordController.GetNextLetter());
	}
}
