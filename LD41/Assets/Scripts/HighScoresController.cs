using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Controller for high scores system
/// </summary>
public class HighScoresController : MonoBehaviour
{
	/// <summary>
	/// Container class for scores
	/// </summary>
	class ScoresContainer
	{
		/// <summary>
		/// The scores
		/// </summary>
		public List<int> Scores = new List<int>();
	}

	/// <summary>
	/// Key name for loading/saving the high scores
	/// </summary>
	private const string HighScoresKey = "HighScores";

	/// <summary>
	/// Container for high scores UI elements
	/// </summary>
	[SerializeField]
	private GameObject _container;

	/// <summary>
	/// The score entry display fields
	/// </summary>
	[SerializeField]
	private List<TextMeshProUGUI> _scoreEntries;

	/// <summary>
	/// The scores
	/// </summary>
	private readonly List<int> _scores = new List<int>();

	/// <summary>
	/// Show the high scores UI
	/// </summary>
	public void ShowHighScores()
	{
		_container.SetActive(true);
	}

	/// <summary>
	/// Hide the high scores UI
	/// </summary>
	public void HideHighScores()
	{
		_container.SetActive(false);
	}

	/// <summary>
	/// Try to add a new score to the high scores
	/// </summary>
	/// <param name="score"></param>
	public void TryAddScore(int score)
	{
		if (_scores.Count >= _scoreEntries.Count)
		{
			var minScore = _scores.Min();
			if (score > minScore)
			{
				_scores.Remove(minScore);
				_scores.Add(score);
				SaveHighScores();
				UpdateScoreEntires();
			}
		}
		else
		{
			_scores.Add(score);
			SaveHighScores();
			UpdateScoreEntires();
		}
	}

	/// <summary>
	/// Updates the score display entries with the newest scores
	/// </summary>
	private void UpdateScoreEntires()
	{
		// Ensure the scores are sorted
		_scores.OrderByDescending(x => x);
		// Update the entries
		for (int i = 0; i < _scoreEntries.Count; ++i)
		{
			if (i < _scores.Count)
			{
				_scoreEntries[i].text = string.Format(CultureInfo.CurrentCulture,
					"{1}. {0:n0}", _scores[i], (i+1));
			}
			else
			{
				_scoreEntries[i].text = "";
			}
		}
	}

	/// <summary>
	/// Self-initialization
	/// </summary>
	private void Awake()
	{
		LoadHighScores();
	}

	/// <summary>
	/// Loads the high scores
	/// </summary>
	private void LoadHighScores()
	{
		_scores.Clear();
		var data = PlayerPrefs.GetString(HighScoresKey);
		if (string.IsNullOrWhiteSpace(data))
		{
			return;
		}

		var container = JsonUtility.FromJson<ScoresContainer>(data);
		_scores.AddRange(container.Scores);
		UpdateScoreEntires();
	}

	/// <summary>
	/// Saves the high scores
	/// </summary>
	private void SaveHighScores()
	{
		var data = JsonUtility.ToJson(new ScoresContainer {
			Scores = _scores
		});
		PlayerPrefs.SetString(HighScoresKey, data);
	}
}
