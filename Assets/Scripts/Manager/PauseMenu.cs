using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	[Header("Buttons")]
	[SerializeField] private Button resumeButton;
	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Button quitButton;

	private void Start()
	{
		// Đăng ký sự kiện cho các nút
		if (resumeButton != null)
			resumeButton.onClick.AddListener(ResumeGame);

		if (mainMenuButton != null)
			mainMenuButton.onClick.AddListener(GoToMainMenu);

		if (quitButton != null)
			quitButton.onClick.AddListener(QuitGame);
	}

	private void ResumeGame()
	{
		GameManager.Instance.ResumeGame();
	}

	private void GoToMainMenu()
	{
		GameManager.Instance.LoadMainMenu();
	}

	private void QuitGame()
	{
		GameManager.Instance.QuitGame();
	}
}