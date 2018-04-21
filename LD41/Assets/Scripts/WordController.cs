﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Controller for the word we are trying to spell
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class WordController : MonoBehaviour
{
	/// <summary>
	/// Color to show the obtained letters as
	/// </summary>
	[SerializeField]
	private Color _obtainedColor;

	/// <summary>
	/// The word display
	/// </summary>
	private TextMeshProUGUI _wordDisplay;

	/// <summary>
	/// The letters in the word.
	/// Each entry is a letter and a boolean if we have obtained it or not
	/// </summary>
	private readonly List<Tuple<char, bool>> _letters = new List<Tuple<char, bool>>();

	/// <summary>
	/// Updates the target word
	/// </summary>
	/// <param name="word">The word to target</param>
	public void SetWord(string word)
	{
		if (string.IsNullOrWhiteSpace(word))
		{
			throw new ArgumentException("Can't set the word to be null or empty", nameof(word));
		}

		_letters.Clear();
		var letters = word.ToCharArray();
		foreach (var letter in letters)
		{
			_letters.Add(new Tuple<char, bool>(letter, false));
		}
	}

	/// <summary>
	/// Gets the next letter that needs to be obtained
	/// </summary>
	/// <returns>The next letter</returns>
	public char GetNextLetter()
	{
		// Find the first entry that has not yet been obtained
		var unobtained = _letters.First(entry => !entry.Item2);
		// Return that leter
		return unobtained.Item1;
	}

	/// <summary>
	/// Self-initialization
	/// </summary>
	private void Awake()
	{
		_wordDisplay = GetComponent<TextMeshProUGUI>();
	}

	/// <summary>
	/// Updates the word display to match the letters we have obtained
	/// </summary>
	private void UpdateDisplay()
	{
		var colorHexStr = ColorUtility.ToHtmlStringRGB(_obtainedColor);
		var builder = new StringBuilder();
		foreach (var entry in _letters)
		{
			// Colorize if obtained
			if (entry.Item2)
			{
				builder.Append("<color=");
				builder.Append(colorHexStr);
				builder.Append(">");
			}

			// Append the letter
			builder.Append(entry.Item1);

			// End color if obtained
			if (entry.Item2)
			{
				builder.Append("</color>");
			}
		}

		_wordDisplay.text = builder.ToString();
	}
}