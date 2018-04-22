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
	/// Coroutine for downloading the dictionary online
	/// </summary>
	private Coroutine _dictionaryCoroutine;

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
	/// Initialize with reference to others
	/// </summary>
	private void Start()
	{
		if (_useDictionary)
		{
			// Download the dictionary, which will pick the word
			_dictionaryCoroutine = StartCoroutine(DownloadDictionary());
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
			// TODO: Show ending (leaderboard, etc.)
			Debug.Log("Done!");
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
