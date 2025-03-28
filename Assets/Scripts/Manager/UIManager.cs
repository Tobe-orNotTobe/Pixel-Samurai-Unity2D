using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	[SerializeField] private GameObject pauseMenuUI;
	[SerializeField] private GameObject gameOverUI;
	[SerializeField] private GameObject gameOverButtonsContainer;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		HideAllUI();
	}

	public void HideAllUI()
	{
		if (pauseMenuUI) pauseMenuUI.SetActive(false);
		if (gameOverUI) gameOverUI.SetActive(false);
		if (gameOverButtonsContainer) gameOverButtonsContainer.SetActive(false);
	}

	public void ShowPauseMenu(bool show)
	{
		if (pauseMenuUI) pauseMenuUI.SetActive(show);
	}

	public void ShowGameOver(bool showButtons = false)
	{
		if (gameOverUI) gameOverUI.SetActive(true);
		if (gameOverButtonsContainer) gameOverButtonsContainer.SetActive(showButtons);
	}

	public void ShowGameOverButtons()
	{
		if (gameOverButtonsContainer) gameOverButtonsContainer.SetActive(true);
	}
}