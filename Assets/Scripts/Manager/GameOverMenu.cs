using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
	[Header("Buttons")]
	[SerializeField] private Button tryAgainButton;
	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Button quitButton;

	[Header("Menu Components")]
	[SerializeField] private GameObject buttonsContainer; 

	private void Start()
	{
		// Register event handlers for buttons
		if (tryAgainButton != null)
			tryAgainButton.onClick.AddListener(RestartGame);

		if (mainMenuButton != null)
			mainMenuButton.onClick.AddListener(GoToMainMenu);

		if (quitButton != null)
			quitButton.onClick.AddListener(QuitGame);

		if (buttonsContainer != null)
			buttonsContainer.SetActive(false);
	}

	private void RestartGame()
	{
		GameManager.Instance.LoadScene(1);
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