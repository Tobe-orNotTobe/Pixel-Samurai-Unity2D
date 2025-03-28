using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[Header("Player")]
	[SerializeField] private GameObject playerPrefab;

	[Header("Game States")]
	private bool isGamePaused = false;
	private bool isGameOver = false;

	[Header("Game Over Settings")]
	[SerializeField] private float delayBeforeShowingButtons = 2f;

	private const int MAIN_MENU_SCENE = 0;
	private const int SCENE_1 = 1;
	private const int SCENE_2 = 2;

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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
		{
			if (isGamePaused)
				ResumeGame();
			else
				PauseGame();
		}
	}

	#region Gameplay Management

	public void StartNewGame()
	{
		isGameOver = false;
		UIManager.Instance.HideAllUI();

		// Set the flag to reset health
		if (PlayerDataManager.Instance != null)
		{
			PlayerDataManager.Instance.ShouldResetHealth = true;
		}

		SceneManager.LoadSceneAsync(SCENE_1);
	}

	public void Respawn()
	{
		PlayerDied();
	}

	public void PlayerDied()
	{
		isGameOver = true;
		UIManager.Instance.ShowGameOver();
		StartCoroutine(ShowGameOverButtonsDelayed());
	}

	private IEnumerator ShowGameOverButtonsDelayed()
	{
		yield return new WaitForSeconds(delayBeforeShowingButtons);
		UIManager.Instance.ShowGameOverButtons();
	}
	#endregion

	#region Menu Management

	public void PauseGame()
	{
		isGamePaused = true;
		Time.timeScale = 0f;
		UIManager.Instance.ShowPauseMenu(true);
	}

	public void ResumeGame()
	{
		isGamePaused = false;
		Time.timeScale = 1f;
		UIManager.Instance.ShowPauseMenu(false);
	}

	public void LoadMainMenu()
	{
		Time.timeScale = 1f;
		UIManager.Instance.HideAllUI();
		isGameOver = false;
		isGamePaused = false;

		if (PlayerDataManager.Instance != null)
		{
			PlayerDataManager.Instance.ShouldResetHealth = true;
		}

		SceneManager.LoadSceneAsync(MAIN_MENU_SCENE);
	}

	public void LoadScene(int sceneIndex)
	{
		Time.timeScale = 1f;
		UIManager.Instance.HideAllUI();
		isGameOver = false;
		isGamePaused = false;
		SceneManager.LoadSceneAsync(sceneIndex);
	}

	public void QuitGame()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}

	#endregion
}