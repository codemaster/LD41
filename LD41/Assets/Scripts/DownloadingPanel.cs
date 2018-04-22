using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Downloading panel to give feedback as the dictionary downloads
/// </summary>
public class DownloadingPanel : MonoBehaviour
{
	/// <summary>
	/// Container for holding the panel's elements
	/// </summary>
	[SerializeField]
	private GameObject _container;

	/// <summary>
	/// Slider to use as the progress bar
	/// </summary>
	[SerializeField]
	private Slider _progressBar;

	/// <summary>
	/// The download text
	/// </summary>
	[SerializeField]
	private TextMeshProUGUI _downloadText;

	/// <summary>
	/// The animation curve to make the text bounce around
	/// </summary>
	[SerializeField]
	private AnimationCurve _textFloating;

	/// <summary>
	/// The request that the panel is monitoring
	/// </summary>
	private UnityWebRequest _monitoringRequest;

	/// <summary>
	/// Shows the downloading panel
	/// </summary>
	public void ShowPanel(UnityWebRequest request)
	{
		_monitoringRequest = request;
		_container.SetActive(true);
	}

	/// <summary>
	/// Hides the downloading panel
	/// </summary>
	public void HidePanel()
	{
		_monitoringRequest = null;
		_container.SetActive(false);
	}

	/// <summary>
	/// Update per frame
	/// </summary>
	private void Update()
	{
		// Monitor the download request
		if (_monitoringRequest != null)
		{
			_progressBar.value = _monitoringRequest.downloadProgress;
			if (_monitoringRequest.downloadProgress >= 1f)
			{
				_monitoringRequest = null;
			}
		}

		// Move the text around
		_downloadText.transform.position = new Vector3(_downloadText.transform.position.x,
			_downloadText.transform.position.y + _textFloating.Evaluate(Time.time),
			_downloadText.transform.position.z);
	}
}
