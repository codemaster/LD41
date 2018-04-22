using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
using Random = UnityEngine.Random;

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
	/// The enemy controller
	/// </summary>
	[Inject]
	private EnemyController _enemyController;

	/// <summary>
	/// The high scores controller
	/// </summary>
	[Inject]
	private HighScoresController _highScoresController;

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
	/// Whether to use the dictionary or not
	/// </summary>
	[SerializeField]
	private bool _useDictionary;

	/// <summary>
	/// The number of words to have the player try to spell
	/// </summary>
	[SerializeField]
	private int _numWords = 5;

	/// <summary>
	/// The words available to play with
	/// </summary>
	[SerializeField]
	private List<string> _words;

	/// <summary>
	/// The words that were already used
	/// </summary>
	private readonly HashSet<string> _usedWords = new HashSet<string>();

	/// <summary>
	/// The URL of the dictionary to download if configured to do so
	/// </summary>
	private const string DictionaryUrl = "https://raw.githubusercontent.com/dwyl/english-words/master/words_alpha.txt";

	/// <summary>
	/// Go to the next letter
	/// </summary>
	public void NextLetter()
	{
		// If the word is completed, start a new word
		if (_wordController.IsCompleted())
		{
			SetNextWord();
			return;
		}

		// Spawn more enemies for the next letter
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
	/// Play again!
	/// </summary>
	public void PlayAgain()
	{
		// Play the sound
		_soundController.PlaySFX(_soundController.PlayAgain);
		// Hide the high scores UI
		_highScoresController.HideHighScores();
		// Reset our score
		_scoreController.Score = 0;
		// Clear the used words
		_usedWords.Clear();
		// Pick the next word
		SetNextWord();
	}

	/// <summary>
	/// Initialize with reference to others
	/// </summary>
	private void Start()
	{
		if (_useDictionary)
		{
			// Download the dictionary, which will pick the word
			StartCoroutine(DownloadDictionary());
		}
		else
		{
			// Pick the next (first) word
			SetNextWord();
		}
	}

	/// <summary>
	/// Downloads the dictionary if possible and assigns our word listing to its contents
	/// </summary>
	/// <returns>Coroutine enumerator</returns>
	private IEnumerator DownloadDictionary()
	{
		// Request the dictionary
		using (var request = UnityWebRequest.Get(DictionaryUrl))
		{
			yield return request.SendWebRequest();

			// Check if an error occurred
			if (request.isNetworkError || request.isHttpError)
			{
				Debug.LogError($"Unable to download dictionary: {request.error}", this);
				yield return null;
			}

			// Obtain the text of the dictionary
			var data = request.downloadHandler.text;

			// Clear our word listing
			_words.Clear();
			// Append each of the non-empty lines in the dictionary to the word listing
			_words.AddRange(data.Split(new[] { "\r\n", "\r", "\n" },
				StringSplitOptions.RemoveEmptyEntries));
		}

		// Pick the next (first) word
		SetNextWord();
	}

	/// <summary>
	/// Sets the next word to spell out
	/// </summary>
	private void SetNextWord()
	{
		// Check if we have used all of the words
		if (_usedWords.Count >= Math.Min(_words.Count, _numWords))
		{
			// Try to add our final score to the high scores
			_highScoresController.TryAddScore(_scoreController.Score);
			// Show the high scores UI
			_highScoresController.ShowHighScores();
			return;
		}

		// Pick unused random word
		string word;
		do {
			var randomIndex = Random.Range(0, _words.Count);
			word = _words[randomIndex];
		} while (string.IsNullOrEmpty(word) || _usedWords.Contains(word));

		// Set the word
		_wordController.SetWord(word);
		// Add the word to the used words listing
		_usedWords.Add(word);
		// Spawn the first enemy
		_enemyController.CreateEnemy(_wordController.GetNextLetter());
	}
}
