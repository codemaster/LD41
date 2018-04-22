using System.Globalization;
using TMPro;
using UnityEngine;

/// <summary>
/// Controller for the score of the game
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreController : MonoBehaviour
{
	/// <summary>
	/// Score increment per letter obtained
	/// </summary>
	[SerializeField]
	public int ScorePerLetterIncrement = 10;

	/// <summary>
	/// Score decremented per enemy shot that was not the letter
	/// </summary>
	[SerializeField]
	public int ScoreDecrementPerBadEnemy = 5;

	/// <summary>
	/// The score display
	/// </summary>
	private TextMeshProUGUI _scoreDisplay;

	/// <summary>
	/// The score
	/// </summary>
	[SerializeField]
	private int _score;

	/// <summary>
	/// Score getter & setter
	/// </summary>
	public int Score
	{
		get { return _score; }
		set {
			_score = value;
			UpdateScoreDisplay();
		}
	}

	/// <summary>
	/// Self-initialization
	/// </summary>
	private void Awake()
	{
		_scoreDisplay = GetComponent<TextMeshProUGUI>();
		UpdateScoreDisplay();
	}

	/// <summary>
	/// Updates the score display
	/// </summary>
	private void UpdateScoreDisplay()
	{
		_scoreDisplay.text = string.Format(CultureInfo.CurrentCulture, "{0:n0}", _score);
	}
}
