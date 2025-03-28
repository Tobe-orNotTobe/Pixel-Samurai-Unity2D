using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[Header("Buttons")]
	[SerializeField] private Button startGameButton;
	[SerializeField] private Button quitButton;

	private void Start()
	{
		// Đăng ký sự kiện cho các nút
		if (startGameButton != null)
			startGameButton.onClick.AddListener(StartGame);

		if (quitButton != null)
			quitButton.onClick.AddListener(QuitGame);
	}

	private void StartGame()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}

	private void QuitGame()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}